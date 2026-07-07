using Application.Tasks.DTOs;
using Domain.Entities.Tasks;

namespace Application.Tasks.Interfaces;

public interface ITaskService
{
    Task<TaskResponse?> GetByIdAsync(Guid tenantId, Guid taskId, CancellationToken ct);
    Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskResponse>> GetByAssigneeIdAsync(
        Guid tenantId,
        Guid assigneeId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskResponse>> GetByReporterIdAsync(
        Guid tenantId,
        Guid reporterId,
        CancellationToken ct
    );
    Task<IEnumerable<TaskResponse>> GetByStatusAsync(
        Guid tenantId,
        TaskItemStatus status,
        CancellationToken ct
    );
    Task<IEnumerable<TaskResponse>> GetByPriorityAsync(
        Guid tenantId,
        TaskPriority priority,
        CancellationToken ct
    );
    Task AddAsync(Guid tenantId, Guid createdBy, CreateTaskRequest taskItem, CancellationToken ct);
    Task UpdateAsync(
        UpdateTaskRequest taskItem,
        Guid taskId,
        Guid tenantId,
        Guid updatedBy,
        CancellationToken ct
    );
    Task DeleteAsync(Guid tenantId, Guid taskId, Guid deletedBy, CancellationToken ct);
}
