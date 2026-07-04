using Domain.Entities.Common;
using Domain.Entities.Projects;

namespace Domain.Entities;

public class ApplicationUser : IAuditable
{
    public Guid Id { get; private init; }
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public Guid PrimaryTenantId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public ICollection<UserTenantRole> TenantRoles { get; private set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public ICollection<ProjectMember> ProjectMemberships { get; private set; } = [];

    public static ApplicationUser Create(
        string userName,
        string email,
        string displayName,
        Guid primaryTenantId
    ) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            DisplayName = displayName,
            PrimaryTenantId = primaryTenantId,
        };

    public void SetPasswordHash(string hash) => PasswordHash = hash;

    public void SetPrimaryTenantId(Guid tenantId) => PrimaryTenantId = tenantId;
}
