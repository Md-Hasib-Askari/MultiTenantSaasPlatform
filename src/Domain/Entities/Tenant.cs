using System.Text.Json;
using Domain.Enums;

namespace Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; private init; }
        public string Slug { get; private init; } = null!;
        public string Name { get; private init; } = null!;
        public PlanTier Plan { get; private set; }
        public DateTimeOffset PlanExpiresAt { get; private set; }
        public TenantStatus Status { get; private set; }
        public IsolationMode IsolationMode { get; private set; }
        public string BillingEmail { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private init; }
        public DateTimeOffset DeletedAt { get; private set; }
        public JsonDocument Settings { get; private set; } = null!;

        private Tenant() { }

        public static Tenant Create(string slug, string name, PlanTier plan = PlanTier.FREE)
        {
            // TODO: guard inputs, set Id, CreatedAt, Status = Active, etc.
            return null!;
        }

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
}
