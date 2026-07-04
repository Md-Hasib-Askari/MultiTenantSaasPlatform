using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public ProjectConfiguration() { }

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder
            .HasOne(p => p.Tenant)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
