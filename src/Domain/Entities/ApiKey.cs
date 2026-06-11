using Domain.Entities.Common;
using Domain.Interfaces;

namespace Domain.Entities;

public class ApiKey : BaseAudit, ITenantScoped
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string KeyHash { get; private set; } = string.Empty;
    public string KeyPrefix { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastUsedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    private ApiKey() { }

    public static ApiKey Create(
        Guid tenantId,
        string name,
        string keyHash,
        string keyPrefix,
        int? expiryDays
    ) =>
        new()
        {
            TenantId = tenantId,
            Name = name,
            KeyHash = keyHash,
            KeyPrefix = keyPrefix,
            ExpiresAt = expiryDays is int days ? DateTimeOffset.UtcNow.AddDays(days) : null,
        };
}
