using Domain.Entities.Common;
using Domain.Entities.Projects;
using Domain.Entities.Tasks;
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
    public string Name { get; private set; } = null!;
    public PlanTier Plan { get; private set; }
    public DateTimeOffset? PlanExpiresAt { get; private set; }
    public TenantStatus Status { get; private set; }
    public string BillingEmail { get; private set; } = null!;
    public TenantSettings Settings { get; private set; } = null!;

    public ICollection<Project> Projects { get; private set; } = [];
    public ICollection<ProjectMember> ProjectMembers { get; private set; } = [];
    public ICollection<TaskItem> TaskItems { get; private set; } = [];
    public ICollection<UserTenantRole> UserRoles { get; private set; } = [];
    public ICollection<Invitaiton> Invitaitons { get; private set; } = [];
    public ICollection<ApiKey> ApiKeys { get; private set; } = [];

    private Tenant() { }

    public static Tenant Create(string slug, string name, PlanTier plan = PlanTier.Free)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be empty.", nameof(slug));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        var tenant = new Tenant()
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Name = name,
            Plan = plan,
            PlanExpiresAt = null,
            Status = TenantStatus.Active,
            BillingEmail = string.Empty, // default empty, can be updated later
            CreatedAt = DateTimeOffset.UtcNow,
            Settings = new TenantSettings(),
        };

        return tenant;
    }

    public void ToggleSuspend()
    {
        // TODO: validate that tenant can be suspended, e.g. not already suspended or deleted, etc.
        Status = Status == TenantStatus.Active ? TenantStatus.Suspended : TenantStatus.Active;
    }

    public void ChangeSettings(TenantSettings newSettings)
    {
        Settings = newSettings;
    }

    public void MarkAsDeleted()
    {
        Status = TenantStatus.Deleted;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public void Update(
        string? name = null,
        TenantStatus? tenantStatus = null,
        PlanTier? newPlan = null,
        string? billingEmail = null
    )
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (tenantStatus is not null)
            Status = tenantStatus.Value;

        if (newPlan is not null)
        {
            Plan = newPlan.Value;
            PlanExpiresAt = DateTimeOffset.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(billingEmail))
            BillingEmail = billingEmail;
    }
}
