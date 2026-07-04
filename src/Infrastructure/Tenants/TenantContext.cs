using Domain.Enums;
using Domain.Interfaces;

namespace Infrastructure.Tenants;

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }

    public string TenantSlug { get; private set; } = string.Empty;

    public PlanTier TenantPlan { get; private set; } = PlanTier.Free;

    public bool IsResolved { get; private set; }

    public void SetTenant(TenantInfo info)
    {
        TenantId = info.Id;
        TenantSlug = info.Slug;
        TenantPlan = info.Plan;
        IsResolved = true;
    }
}

public record TenantInfo(
    Guid Id,
    string Slug,
    string Name,
    PlanTier Plan,
    TenantStatus Status,
    DateTimeOffset? DeletedAt = null
);
