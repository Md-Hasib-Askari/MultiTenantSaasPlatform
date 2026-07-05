using Domain.Entities.Tasks;

namespace Application.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid tenantId, Guid taskId, CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct = default
    );
    Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(
        Guid tenantId,
        Guid assigneeId,
        CancellationToken ct = default
    );
    Task<IEnumerable<TaskItem>> GetByReporterIdAsync(
        Guid tenantId,
        Guid reporterId,
        CancellationToken ct = default
    );
    Task<IEnumerable<TaskItem>> GetByStatusAsync(
        Guid tenantId,
        TaskItemStatus status,
        CancellationToken ct = default
    );
    Task<IEnumerable<TaskItem>> GetByPriorityAsync(
        Guid tenantId,
        TaskPriority? priority = null,
        CancellationToken ct = default
    );
    Task AddAsync(TaskItem taskItem, CancellationToken ct = default);
    Task UpdateAsync(TaskItem taskItem, CancellationToken ct = default);
    Task DeleteAsync(Guid taskItemId, Guid deletedBy, CancellationToken ct = default);
}
