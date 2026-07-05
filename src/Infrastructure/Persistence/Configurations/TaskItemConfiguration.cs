using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public TaskItemConfiguration() { }

    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(ti => ti.Id);

        builder.Property(ti => ti.Title).IsRequired().HasMaxLength(200);
        builder.Property(ti => ti.Description).HasMaxLength(2000);
        builder.Property(ti => ti.Status).IsRequired();
        builder.Property(ti => ti.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(ti => ti.Priority).HasConversion<string>().HasMaxLength(50);
        builder.Property(ti => ti.EstimatedHours).HasPrecision(18, 2);
        builder.Property(ti => ti.CreatedAt).IsRequired();

        builder
            .HasIndex(ti => new
            {
                ti.TenantId,
                ti.ProjectId,
                ti.Title,
            })
            .IsUnique();
        builder.HasIndex(ti => new { ti.TenantId, ti.AssigneeId });
        builder.HasIndex(ti => new { ti.TenantId, ti.ReporterId });
        builder.HasIndex(ti => new { ti.TenantId, ti.Status });
        builder.HasIndex(ti => new { ti.TenantId, ti.Priority });
        builder.HasIndex(ti => new { ti.ProjectId, ti.Sequence });

        builder
            .HasOne(ti => ti.Tenant)
            .WithMany(t => t.TaskItems)
            .HasForeignKey(ti => ti.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ti => ti.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(ti => ti.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ti => ti.Assignee)
            .WithMany()
            .HasForeignKey(ti => ti.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(ti => ti.Reporter)
            .WithMany()
            .HasForeignKey(ti => ti.ReporterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
