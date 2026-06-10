using System.Reflection;
using Domain.Entities;
using Domain.Interfaces;
using infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
    : DbContext(options)
{
    private readonly ITenantContext _itenantContext = tenantContext;
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ApiKey> ApiKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());

        modelBuilder
            .Entity<User>()
            .HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId);

        ApplyTenantQueryFilters(modelBuilder);
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
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _itenantContext.TenantId);
    }
}
