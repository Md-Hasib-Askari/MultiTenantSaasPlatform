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

    public Tenant Tenant { get; set; } = null!;

    private ApiKey() { }
}
