using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Domain.Entities.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tasks;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(TaskItem taskItem, CancellationToken ct = default)
    {
        await _db.TaskItems.AddAsync(taskItem, ct);
    }

    public async Task DeleteAsync(Guid taskItemId, Guid deletedBy, CancellationToken ct = default)
    {
        var task =
            await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskItemId, ct)
            ?? throw new InvalidOperationException($"Task with ID {taskItemId} not found.");

        task.MarkAsDeleted(deletedBy);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(
        Guid tenantId,
        Guid assigneeId,
        CancellationToken ct = default
    )
    {
        return await _db
            .TaskItems.Where(t => t.TenantId == tenantId && t.AssigneeId == assigneeId)
            .ToListAsync(ct);
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid tenantId,
        Guid taskId,
        CancellationToken ct = default
    )
    {
        return await _db.TaskItems.FirstOrDefaultAsync(
            t => t.TenantId == tenantId && t.Id == taskId,
            ct
        );
    }

    public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(
        Guid tenantId,
        TaskPriority? priority = null,
        CancellationToken ct = default
    )
    {
        return await _db
            .TaskItems.Where(t => t.TenantId == tenantId && t.Priority == priority)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct = default
    )
    {
        return await _db
            .TaskItems.Where(t => t.TenantId == tenantId && t.ProjectId == projectId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TaskItem>> GetByReporterIdAsync(
        Guid tenantId,
        Guid reporterId,
        CancellationToken ct = default
    )
    {
        return await _db
            .TaskItems.Where(t => t.TenantId == tenantId && t.ReporterId == reporterId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(
        Guid tenantId,
        TaskItemStatus status,
        CancellationToken ct = default
    )
    {
        return await _db
            .TaskItems.Where(t => t.TenantId == tenantId && t.Status == status)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(TaskItem taskItem, CancellationToken ct = default)
    {
        _db.TaskItems.Update(taskItem);
        await _db.SaveChangesAsync(ct);
    }
}
