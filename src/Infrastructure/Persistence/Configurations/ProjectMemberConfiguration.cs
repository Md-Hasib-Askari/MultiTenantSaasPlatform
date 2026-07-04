using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public ProjectMemberConfiguration() { }

    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.HasIndex(pm => new { pm.ProjectId, pm.UserId }).IsUnique();

        builder
            .HasOne(pm => pm.Tenant)
            .WithMany(t => t.ProjectMembers)
            .HasForeignKey(pm => pm.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
