using Domain.Entities.Common;
using Domain.Interfaces;

namespace Domain.Entities.Projects;

public class Project : BaseAudit, ITenantScoped
{
    public Guid Id { get; private init; }
    public Guid TenantId { get; private init; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Color { get; set; } = "#63B3ED";

    public Tenant Tenant { get; private set; } = null!;
    public ICollection<TaskItem> Tasks { get; private set; } = [];
    public ICollection<ProjectMember> Members { get; private set; } = [];

    private Project() { }

    public static Project Create(Guid tenantId, string name) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
