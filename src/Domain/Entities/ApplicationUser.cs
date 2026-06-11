using Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable
{
    public string DisplayName { get; private set; } = string.Empty;
    public Guid PrimaryTenantId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public ICollection<UserTenantRole> TenantRoles { get; set; } = [];
}
