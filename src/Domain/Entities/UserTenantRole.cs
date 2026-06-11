using Domain.Interfaces;

namespace Domain.Entities;

public class UserTenantRole : ITenantScoped
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Role { get; private set; } = TenantRole.Member;

    public ApplicationUser User { get; private set; } = null!;
    public Tenant Tenant { get; private set; } = null!;

    private UserTenantRole() { }

    public static UserTenantRole Create(
        Guid userId,
        Guid tenantId,
        string role = TenantRole.Member
    ) =>
        new()
        {
            UserId = userId,
            TenantId = tenantId,
            Role = role,
        };
}

public static class TenantRole
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Member = "Member";
    public const string Viewer = "Viewer";

    public static bool IsValid(string role) => role is Owner or Admin or Member or Viewer;
}
