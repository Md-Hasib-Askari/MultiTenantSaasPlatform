using Application.Common;
using Domain.Entities.Tasks;

namespace Application.Tasks.DTOs;

[AtLeastOneRequired(
    "Title",
    "Description",
    "Status",
    "Priority",
    "AssigneeId",
    "ReporterId",
    "DueDate",
    "EstimatedHours",
    "Sequence"
)]
public record UpdateTaskDto(
    string? Title,
    string? Description,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    Guid? AssigneeId,
    Guid? ReporterId,
    DateTimeOffset? DueDate,
    decimal? EstimatedHours,
    int? Sequence
);
