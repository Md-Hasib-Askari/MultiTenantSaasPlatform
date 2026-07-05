using System.ComponentModel.DataAnnotations;
using Domain.Entities.Tasks;

namespace Application.Tasks.DTOs;

public record CreateTaskDto(
    [Required] Guid ProjectId,
    [Required] string Title,
    string? Description,
    TaskItemStatus Status = TaskItemStatus.NotStarted,
    TaskPriority? Priority = null,
    Guid? AssigneeId = null,
    Guid? ReporterId = null,
    DateTimeOffset? DueDate = null,
    decimal? EstimatedHours = null,
    int Sequence = 0
);
