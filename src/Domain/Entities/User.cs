using Domain.Entities.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class User : BaseAudit, ITenantScoped
{
    public Guid Id { get; private init; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserStatus Status { get; private set; }

    public Guid TenantId { get; private init; }
    public Tenant Tenant { get; private set; } = null!;

    private User() { }

    public static User Create(Guid tenantId, string email, string firstName, string lastName) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
        };

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsDeleted()
    {
        Status = UserStatus.Deleted;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
