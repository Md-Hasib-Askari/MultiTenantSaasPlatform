using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public TenantConfiguration() { }

    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.BillingEmail).IsRequired().HasMaxLength(200);

        builder.Property(t => t.Plan).IsRequired().HasMaxLength(50).HasConversion<string>();
        builder.Property(t => t.Status).IsRequired().HasMaxLength(50).HasConversion<string>();
        builder
            .Property(t => t.IsolationMode)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.OwnsOne(
            t => t.Settings,
            s =>
            {
                s.ToJson();
                s.Property(x => x.TimeZone).HasDefaultValue("UTC");
                s.Property(x => x.DateFormat).HasDefaultValue("yyyy-MM-dd");
            }
        );
    }
}
