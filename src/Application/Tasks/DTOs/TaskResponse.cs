using Domain.Entities.Tasks;

namespace Application.Tasks.DTOs;

public record TaskResponse(
    Guid Id,
    Guid TenantId,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority? Priority,
    Guid? AssigneeId,
    Guid? ReporterId,
    DateTimeOffset? DueDate,
    decimal? EstimatedHours,
    int Sequence,
    DateTimeOffset CreatedAt,
    Guid CreatedById,
    DateTimeOffset? UpdatedAt,
    Guid? UpdatedById
);
