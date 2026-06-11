using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitaiton>
{
    public void Configure(EntityTypeBuilder<Invitaiton> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasOne(i => i.Tenant).WithMany().HasForeignKey(i => i.TenantId);
    }
}
