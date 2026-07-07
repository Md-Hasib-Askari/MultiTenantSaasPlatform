using Domain.Entities.Common;
using Domain.Entities.Projects;
using Domain.Enums;

namespace Domain.Entities;

public class ApplicationUser : IAuditable
{
    public Guid Id { get; private init; }
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public Guid PrimaryTenantId { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

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

    public void SetStatus(UserStatus status) => Status = status;

    public void UpdateProfile(string? displayName, string? email)
    {
        if (displayName is not null)
            DisplayName = displayName;
        if (email is not null)
            Email = email;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsDeleted()
    {
        Status = UserStatus.Deleted;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
