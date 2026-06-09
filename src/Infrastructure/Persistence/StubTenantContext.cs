using Domain.Enums;
using Domain.Interfaces;

namespace Infrastructure.Persistence;

public class StubTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Parse("11111111-1111-1111-1111-111111111111");
    public string TenantSlug => "sub-tenant";
    public PlanTier TenantPlan => PlanTier.Free;
    public bool IsResolved => true;
}
