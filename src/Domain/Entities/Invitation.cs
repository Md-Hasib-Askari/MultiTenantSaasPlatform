using Domain.Entities.Common;
using Domain.Interfaces;

namespace Domain.Entities;

public class Invitaiton : ITenantScoped, IAuditable
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = TenantRole.Member;
    public string TokenHash { get; private set; } = string.Empty;
    public bool Accepted { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    private Invitaiton() { }

    public static Invitaiton Create(
        Guid tenantId,
        string email,
        string tokenHash,
        string role = TenantRole.Member,
        int expiryDays = 7
    ) =>
        new()
        {
            TenantId = tenantId,
            Email = email,
            TokenHash = tokenHash,
            Role = role,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(expiryDays),
        };

    public void Accept() => Accepted = true;

    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
}
