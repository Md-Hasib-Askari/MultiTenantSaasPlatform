using Domain.Entities.Common;
using Domain.Interfaces;

namespace Domain.Entities;

public class TaskItem : BaseAudit, ITenantScoped
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    // TODO: Complete the TaskItem entity
}
