using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Name).IsRequired().HasMaxLength(100);
        builder.Property(k => k.KeyHash).IsRequired().HasMaxLength(256);
        builder.Property(k => k.KeyPrefix).IsRequired().HasMaxLength(20);

        builder.HasIndex(k => k.KeyHash).IsUnique();

        builder
            .HasOne(k => k.Tenant)
            .WithMany()
            .HasForeignKey(k => k.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
