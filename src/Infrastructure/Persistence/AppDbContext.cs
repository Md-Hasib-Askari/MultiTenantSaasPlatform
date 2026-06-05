using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Project>()
            .HasOne(p => p.Tenant)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TenantId);
    }
}
