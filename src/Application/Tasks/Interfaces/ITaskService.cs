using Application.Tasks.DTOs;
using Domain.Entities.Tasks;

namespace Application.Tasks.Interfaces;

public interface ITaskService
{
    Task<TaskItemDto?> GetByIdAsync(Guid tenantId, Guid taskId, CancellationToken ct);
    Task<IEnumerable<TaskItemDto>> GetByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskItemDto>> GetByAssigneeIdAsync(
        Guid tenantId,
        Guid assigneeId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskItemDto>> GetByReporterIdAsync(
        Guid tenantId,
        Guid reporterId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskItemDto>> GetByStatusAsync(
        Guid tenantId,
        TaskItemStatus status,
        CancellationToken ct
    );
    Task<IEnumerable<TaskItemDto>> GetByPriorityAsync(
        Guid tenantId,
        TaskPriority priority,
        CancellationToken ct
    );
    Task AddAsync(Guid tenantId, Guid createdBy, CreateTaskDto taskItem, CancellationToken ct);
    Task UpdateAsync(
        UpdateTaskDto taskItem,
        Guid taskId,
        Guid tenantId,
        Guid updatedBy,
        CancellationToken ct
    );
    Task DeleteAsync(Guid tenantId, Guid taskId, Guid deletedBy, CancellationToken ct);
}
