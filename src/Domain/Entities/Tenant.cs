using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

public class TenantSettings
{
    public string TimeZone { get; set; } = "UTC";
    public string DateFormat { get; set; } = "yyyy-MM-dd";
}

public class Tenant : BaseAudit
{
    public Guid Id { get; private init; }
    public string Slug { get; private init; } = null!;
    public string Name { get; private init; } = null!;
    public PlanTier Plan { get; private set; }
    public DateTimeOffset? PlanExpiresAt { get; private set; } = null!;
    public TenantStatus Status { get; private set; }
    public IsolationMode IsolationMode { get; private set; }
    public string BillingEmail { get; private set; } = null!;
    public TenantSettings Settings { get; private set; } = null!;

    private Tenant() { }

    public static Tenant Create(string slug, string name, PlanTier plan = PlanTier.FREE) =>
        // TODO: guard inputs, set Id, CreatedAt, Status = Active, etc.
        new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Name = name,
            Plan = plan,
            PlanExpiresAt = DateTimeOffset.UtcNow.AddMonths(1), // default to 1 month trial for paid plans
            Status = TenantStatus.ACTIVE,
            IsolationMode = IsolationMode.SHARED, // default isolation mode
            BillingEmail = string.Empty, // default empty, can be updated later
            CreatedAt = DateTimeOffset.UtcNow,
            Settings = new TenantSettings() { DateFormat = "UTC", TimeZone = "yyyy-MM-dd" }, // default settings
        };

    public void Suspend()
    {
        // TODO: validate that tenant can be suspended, e.g. not already suspended or deleted, etc.
        Status = TenantStatus.SUSPENDED;
    }

    public void ChangePlan(PlanTier newPlan, DateTimeOffset expiresAt)
    {
        Plan = newPlan;
        PlanExpiresAt = expiresAt;
    }

    public void MarkAsDeleted()
    {
        Status = TenantStatus.DELETED;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
