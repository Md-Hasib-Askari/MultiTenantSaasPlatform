using Domain.Enums;

namespace Domain.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
    string TenantSlug { get; }
    PlanTier TenantPlan { get; }
}
