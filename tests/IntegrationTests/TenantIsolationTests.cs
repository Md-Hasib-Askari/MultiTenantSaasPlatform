using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class TestableTenantContext : ITenantContext
{
    public Guid TenantId { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public PlanTier TenantPlan { get; set; } = PlanTier.Free;
    public bool IsResolved { get; set; }
}

public class TenantIsolationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("mtsp_test")
        .WithUsername("mtsp")
        .WithPassword("mtsp")
        .Build();

    private string _connectionString = null!;
    private string _appUserConnString = null!;
    private Guid _tenantAId;
    private Guid _tenantBId;

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _connectionString = _postgres.GetConnectionString();

        // Build an app-user connection string (non-superuser) for RLS tests.
        // Superusers bypass RLS even with FORCE ROW LEVEL SECURITY,
        // so we must test with a regular role.
        var builder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Username = "app_user",
            Password = "app_password",
        };
        _appUserConnString = builder.ConnectionString;

        // Run migrations + seed data once for all tests
        await SeedDataAsync();
    }

    private async Task<AppDbContext> CreateDbContextAsync(
        TestableTenantContext tenantContext,
        string? connectionString = null
    )
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString ?? _connectionString)
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new RlsConnectionInterceptor(tenantContext))
            .Options;

        var context = new AppDbContext(options, tenantContext);
        return context;
    }

    private async Task SeedDataAsync()
    {
        // Run migrations as superuser
        var tenantContext = new TestableTenantContext { IsResolved = false };
        await using var context = await CreateDbContextAsync(tenantContext);
        await context.Database.MigrateAsync();

        // Seed tenants via EF (query filters don't block INSERT on Tenants since it's not ITenantScoped)
        var tenantA = Tenant.Create("tenant-a", "Tenant A Corp", PlanTier.Pro);
        var tenantB = Tenant.Create("tenant-b", "Tenant B Inc", PlanTier.Free);

        context.Tenants.Add(tenantA);
        context.Tenants.Add(tenantB);
        await context.SaveChangesAsync();

        _tenantAId = tenantA.Id;
        _tenantBId = tenantB.Id;

        // Insert projects via raw SQL to bypass RLS + query filters during seeding
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var projectA1Id = Guid.NewGuid();
        var projectA2Id = Guid.NewGuid();
        var projectB1Id = Guid.NewGuid();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"""
            INSERT INTO projects (id, name, tenant_id, created_at, updated_at, deleted_at)
            VALUES
            ('{projectA1Id}', 'Project A-1', '{tenantA.Id}', NOW(), NOW(), NULL),
            ('{projectA2Id}', 'Project A-2', '{tenantA.Id}', NOW(), NOW(), NULL),
            ('{projectB1Id}', 'Project B-1', '{tenantB.Id}', NOW(), NOW(), NULL);
            """;
        await cmd.ExecuteNonQueryAsync();

        // Create a non-superuser role that RLS will apply to
        await using var roleCmd = conn.CreateCommand();
        roleCmd.CommandText =
            "DO $$ BEGIN IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'app_user') THEN CREATE ROLE app_user WITH LOGIN PASSWORD 'app_password'; END IF; END $$";
        await roleCmd.ExecuteNonQueryAsync();

        await using var grantDbCmd = conn.CreateCommand();
        grantDbCmd.CommandText = "GRANT CONNECT ON DATABASE mtsp_test TO app_user";
        await grantDbCmd.ExecuteNonQueryAsync();

        await using var grantSchemaCmd = conn.CreateCommand();
        grantSchemaCmd.CommandText = "GRANT USAGE ON SCHEMA public TO app_user";
        await grantSchemaCmd.ExecuteNonQueryAsync();

        await using var grantTablesCmd = conn.CreateCommand();
        grantTablesCmd.CommandText =
            "GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_user";
        await grantTablesCmd.ExecuteNonQueryAsync();

        await using var grantSeqCmd = conn.CreateCommand();
        grantSeqCmd.CommandText =
            "GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO app_user";
        await grantSeqCmd.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task EfQueryFilter_ReturnsOnlyCurrentTenantProjects()
    {
        // Arrange
        var tenantContext = new TestableTenantContext
        {
            TenantId = _tenantAId,
            TenantSlug = "tenant-a",
            TenantPlan = PlanTier.Pro,
            IsResolved = true,
        };

        await using var context = await CreateDbContextAsync(tenantContext);

        // Act
        var projects = await context.Projects.ToListAsync();

        // Assert - only Tenant A's 2 projects returned, not Tenant B's
        Assert.NotEmpty(projects);
        Assert.All(projects, p => Assert.Equal(_tenantAId, p.TenantId));
        Assert.Equal(2, projects.Count);
    }

    [Fact]
    public async Task RawSqlWithRls_ReturnsOnlyCurrentTenantRows()
    {
        // Arrange - connect as non-superuser so RLS is enforced
        await using var conn = new NpgsqlConnection(_appUserConnString);
        await conn.OpenAsync();

        // Set the RLS session variable to Tenant B
        await using var setCmd = conn.CreateCommand();
        setCmd.CommandText = $"SET app.tenant_id = '{_tenantBId}'";
        await setCmd.ExecuteNonQueryAsync();

        // Act - raw SQL query, no EF invloved
        await using var queryCmd = conn.CreateCommand();
        queryCmd.CommandText = "SELECT id, name, tenant_id FROM projects";
        await using var reader = await queryCmd.ExecuteReaderAsync();

        var results = new List<(Guid Id, string Name, Guid TenantId)>();
        while (await reader.ReadAsync())
        {
            results.Add((reader.GetGuid(0), reader.GetString(1), reader.GetGuid(2)));
        }

        // Assert - only Tenant B's 1 row returned
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Equal(_tenantBId, r.TenantId));
        Assert.Single(results);
        Assert.Equal("Project B-1", results[0].Name);
    }

    [Fact]
    public async Task CrossTenant_AccessByForeignKey_ReturnsNull()
    {
        // Arrange - get Tenant B's project ID using Tenant B's context
        Guid projectBId;
        var tenantBContext = new TestableTenantContext
        {
            TenantId = _tenantBId,
            TenantSlug = "tenant-b",
            TenantPlan = PlanTier.Free,
            IsResolved = true,
        };
        await using var contextB = await CreateDbContextAsync(tenantBContext);
        projectBId = (await contextB.Projects.FirstAsync()).Id;

        // Act - try to find Tenant B's project using Tenant A's context
        var tenantAContext = new TestableTenantContext
        {
            TenantId = _tenantAId,
            TenantSlug = "tenant-a",
            TenantPlan = PlanTier.Pro,
            IsResolved = true,
        };
        await using var contextA = await CreateDbContextAsync(tenantAContext);
        var found = await contextA.Projects.FirstOrDefaultAsync(p => p.Id == projectBId);

        // Assert - Tenant A cannot see Tenant B's projects
        Assert.Null(found);
    }

    [Fact]
    public async Task NoTenantContext_ReturnsNoRows()
    {
        // Arrange - connect as non-superuser so RLS is enforced
        await using var conn = new NpgsqlConnection(_appUserConnString);
        await conn.OpenAsync();

        // Set RLS session variable to empty (no tenant)
        await using var setCmd = conn.CreateCommand();
        setCmd.CommandText = $"SET app.tenant_id = '00000000-0000-0000-0000-000000000000'";
        await setCmd.ExecuteNonQueryAsync();

        // Act
        await using var queryCmd = conn.CreateCommand();
        queryCmd.CommandText = "SELECT COUNT(*) FROM projects";
        var count = (long)(await queryCmd.ExecuteScalarAsync())!;

        // Assert - no rows visible without a valid tenant
        Assert.Equal(0L, count);
    }
}
