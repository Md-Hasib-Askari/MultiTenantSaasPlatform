using System.Text.Json.Serialization;
using Domain.Entities.Common;
using Domain.Entities.Projects;
using Domain.Interfaces;

namespace Domain.Entities.Tasks;

public class TaskItem : BaseAudit, ITenantScoped
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; } = TaskItemStatus.NotStarted;
    public TaskPriority? Priority { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public Guid? ReporterId { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public int Sequence { get; private set; }

    public Tenant Tenant { get; private set; } = null!;
    public Project Project { get; private set; } = null!;
    public ApplicationUser? Assignee { get; private set; }
    public ApplicationUser? Reporter { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(
        Guid tenantId,
        Guid projectId,
        string title,
        string? description,
        TaskItemStatus status,
        TaskPriority? priority,
        Guid? assigneeId,
        Guid? reporterId,
        DateTimeOffset? dueDate,
        decimal? estimatedHours,
        int sequence,
        Guid createdById
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        if (sequence < 0)
            throw new ArgumentOutOfRangeException(nameof(sequence), "Sequence cannot be negative.");

        return new()
        {
            TenantId = tenantId,
            ProjectId = projectId,
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            AssigneeId = assigneeId,
            ReporterId = reporterId,
            DueDate = dueDate,
            EstimatedHours = estimatedHours,
            Sequence = sequence,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedById = createdById,
        };
    }

    public void MarkAsDeleted(Guid deletedById)
    {
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedById = deletedById;
    }

    public void Update(
        string? title,
        string? description,
        TaskItemStatus? status,
        TaskPriority? priority,
        Guid? assigneeId,
        Guid? reporterId,
        DateTimeOffset? dueDate,
        decimal? estimatedHours,
        int? sequence,
        Guid updatedById
    )
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            Title = title;
        }

        if (description is not null)
            Description = description;

        if (status is not null)
            Status = status.Value;

        if (priority is not null)
            Priority = priority.Value;

        if (assigneeId is not null)
            AssigneeId = assigneeId;

        if (reporterId is not null)
            ReporterId = reporterId;

        if (dueDate is not null)
            DueDate = dueDate;

        if (estimatedHours is not null)
            EstimatedHours = estimatedHours;

        if (sequence is not null)
        {
            if (sequence < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sequence),
                    "Sequence cannot be negative."
                );

            Sequence = sequence.Value;
        }

        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedById = updatedById;
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemStatus
{
    NotStarted,
    InProgress,
    Completed,
    OnHold,
    Cancelled,
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical,
}
