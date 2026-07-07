using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Domain.Entities.Tasks;

namespace Application.Tasks.Services;

public class TaskService(ITaskRepository taskRepo) : ITaskService
{
    private readonly ITaskRepository _taskRepo = taskRepo;

    public Task AddAsync(Guid tenantId, Guid createBy, CreateTaskRequest taskItem, CancellationToken ct)
    {
        var task = TaskItem.Create(
            tenantId,
            taskItem.ProjectId,
            taskItem.Title,
            taskItem.Description,
            taskItem.Status,
            taskItem.Priority,
            taskItem.AssigneeId,
            taskItem.ReporterId,
            taskItem.DueDate,
            taskItem.EstimatedHours,
            taskItem.Sequence,
            createBy
        );

        return _taskRepo.AddAsync(task, ct);
    }

    public async Task DeleteAsync(Guid tenantId, Guid taskId, Guid deletedBy, CancellationToken ct)
    {
        await _taskRepo.DeleteAsync(taskId, deletedBy, ct);
    }

    public async Task<IEnumerable<TaskResponse>> GetByAssigneeIdAsync(
        Guid tenantId,
        Guid assigneeId,
        CancellationToken ct
    )
    {
        var tasks = await _taskRepo.GetByAssigneeIdAsync(tenantId, assigneeId, ct);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid tenantId, Guid taskId, CancellationToken ct)
    {
        var task = await _taskRepo.GetByIdAsync(tenantId, taskId, ct);

        return task is not null ? MapToDto(task) : null;
    }

    public async Task<IEnumerable<TaskResponse>> GetByPriorityAsync(
        Guid tenantId,
        TaskPriority priority,
        CancellationToken ct
    )
    {
        var tasks = await _taskRepo.GetByPriorityAsync(tenantId, priority, ct);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct
    )
    {
        var tasks = await _taskRepo.GetByProjectIdAsync(tenantId, projectId, ct);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskResponse>> GetByReporterIdAsync(
        Guid tenantId,
        Guid reporterId,
        CancellationToken ct
    )
    {
        var tasks = await _taskRepo.GetByReporterIdAsync(tenantId, reporterId, ct);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskResponse>> GetByStatusAsync(
        Guid tenantId,
        TaskItemStatus status,
        CancellationToken ct
    )
    {
        var tasks = await _taskRepo.GetByStatusAsync(tenantId, status, ct);
        return tasks.Select(MapToDto);
    }

    public async Task UpdateAsync(
        UpdateTaskRequest taskItem,
        Guid taskId,
        Guid tenantId,
        Guid updateBy,
        CancellationToken ct
    )
    {
        var task =
            await _taskRepo.GetByIdAsync(tenantId, taskId, ct)
            ?? throw new InvalidOperationException("Task not found");

        task.Update(
            taskItem.Title,
            taskItem.Description,
            taskItem.Status,
            taskItem.Priority,
            task.AssigneeId,
            task.ReporterId,
            taskItem.DueDate,
            taskItem.EstimatedHours,
            taskItem.Sequence,
            updateBy
        );
        await _taskRepo.UpdateAsync(task, ct);
    }

    private static TaskResponse MapToDto(TaskItem t) =>
        new(
            t.Id,
            t.TenantId,
            t.ProjectId,
            t.Title,
            t.Description,
            t.Status,
            t.Priority,
            t.AssigneeId,
            t.ReporterId,
            t.DueDate,
            t.EstimatedHours,
            t.Sequence,
            t.CreatedAt,
            t.CreatedById,
            t.UpdatedAt,
            t.UpdatedById
        );
}
