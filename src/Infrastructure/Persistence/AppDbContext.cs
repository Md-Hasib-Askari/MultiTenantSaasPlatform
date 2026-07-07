using System.Reflection;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Entities.Projects;
using Domain.Entities.Tasks;
using Domain.Interfaces;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
    : DbContext(options)
{
    private readonly ITenantContext _tenantContext = tenantContext;
    public DbSet<ApplicationUser> Users { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<UserTenantRole> UserTenantRoles { get; set; } = null!;
    public DbSet<Invitaiton> Invitaitons { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<ApiKey> ApiKeys { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<TaskItem> TaskItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UserTenantRoleConfiguration());
        modelBuilder.ApplyConfiguration(new InvitationConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        ConfigureAuditRelationships(modelBuilder);
        ApplyTenantQueryFilters(modelBuilder);
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private static void ConfigureAuditRelationships(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (typeof(ICreateAudit).IsAssignableFrom(clrType))
            {
                modelBuilder
                    .Entity(clrType)
                    .HasOne(nameof(BaseAudit.CreatedBy))
                    .WithMany()
                    .HasForeignKey(nameof(BaseAudit.CreatedById));
            }

            if (typeof(IUpdateAudit).IsAssignableFrom(clrType))
            {
                modelBuilder
                    .Entity(clrType)
                    .HasOne(nameof(BaseAudit.UpdatedBy))
                    .WithMany()
                    .HasForeignKey(nameof(BaseAudit.UpdatedById));
            }

            if (typeof(IDeleteAudit).IsAssignableFrom(clrType))
            {
                modelBuilder
                    .Entity(clrType)
                    .HasOne(nameof(BaseAudit.DeletedBy))
                    .WithMany()
                    .HasForeignKey(nameof(BaseAudit.DeletedById));
            }
        }
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(
                        nameof(ApplyTenantQueryFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, [modelBuilder]);
            }
        }
    }

    private void ApplyTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantScoped
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
    }

    private void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IDeleteAudit).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(
                        nameof(ApplySoftDeleteQueryFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance
                    )!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, [modelBuilder]);
            }
        }
    }

    private void ApplySoftDeleteQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IDeleteAudit
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.DeletedAt == null);
    }
}
