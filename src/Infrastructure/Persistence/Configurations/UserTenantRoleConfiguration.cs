using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserTenantRoleConfiguration : IEntityTypeConfiguration<UserTenantRole>
{
    public void Configure(EntityTypeBuilder<UserTenantRole> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.UserId, r.TenantId }).IsUnique();
        builder.HasOne(r => r.User).WithMany(u => u.TenantRoles).HasForeignKey(r => r.UserId);
        builder.HasOne(r => r.Tenant).WithMany().HasForeignKey(r => r.TenantId);
    }
}
