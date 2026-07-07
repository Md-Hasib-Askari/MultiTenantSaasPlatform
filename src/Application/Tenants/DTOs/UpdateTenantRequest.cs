using Application.Common;
using Domain.Enums;

namespace Application.Tenants.DTOs;

[AtLeastOneRequired(nameof(Name), nameof(TenantStatus), nameof(TenantPlan), nameof(BillingEmail))]
public record UpdateTenantRequest(
    string? Name,
    TenantStatus TenantStatus,
    PlanTier TenantPlan,
    string? BillingEmail
);
