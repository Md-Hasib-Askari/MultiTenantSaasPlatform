using Api.Models;
using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Domain.Entities.Tasks;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskController(
    ITaskService taskService,
    ITenantContext tenantContext,
    IAuthorizationService authorizationService
) : ControllerBase
{
    [Authorize(Policy = "ProjectMemberView")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var task = await taskService.GetByIdAsync(tenantContext.TenantId, id, ct);
        if (task is null)
            return NotFound(new { error = $"Task with ID {id} not found." });

        return Ok(ApiResponse<TaskResponse>.Ok(task));
    }

    [Authorize(Policy = "ProjectMemberView")]
    [HttpGet("by-project/{projectId:guid}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId, CancellationToken ct)
    {
        var tasks = await taskService.GetByProjectIdAsync(tenantContext.TenantId, projectId, ct);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks));
    }

    [HttpGet("by-assignee/{assigneeId}")]
    public async Task<IActionResult> GetByAssigneeId(Guid assigneeId, CancellationToken ct)
    {
        var tasks = await taskService.GetByAssigneeIdAsync(tenantContext.TenantId, assigneeId, ct);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks));
    }

    [HttpGet("by-reporter/{reporterId}")]
    public async Task<IActionResult> GetByReporterId(Guid reporterId, CancellationToken ct)
    {
        var tasks = await taskService.GetByReporterIdAsync(tenantContext.TenantId, reporterId, ct);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks));
    }

    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetByStatus(TaskItemStatus status, CancellationToken ct)
    {
        var tasks = await taskService.GetByStatusAsync(tenantContext.TenantId, status, ct);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks));
    }

    [HttpGet("by-priority/{priority}")]
    public async Task<IActionResult> GetByPriority(TaskPriority priority, CancellationToken ct)
    {
        var tasks = await taskService.GetByPriorityAsync(tenantContext.TenantId, priority, ct);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest dto, CancellationToken ct)
    {
        var userId = User.GetUserId();

        var auth = await authorizationService.AuthorizeAsync(User, dto.ProjectId, "ProjectMemberEdit");
        if (!auth.Succeeded)
            return Forbid();

        await taskService.AddAsync(tenantContext.TenantId, userId, dto, ct);
        return Ok(ApiResponse<object>.Ok("Task created successfully"));
    }

    [Authorize(Policy = "TaskUpdate")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await taskService.UpdateAsync(dto, id, tenantContext.TenantId, userId, ct);
        return Ok(ApiResponse<object>.Ok("Task updated successfully"));
    }

    [Authorize(Policy = "ProjectMemberAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await taskService.DeleteAsync(tenantContext.TenantId, id, userId, ct);
        return Ok(ApiResponse<object>.Ok("Task deleted successfully"));
    }
}
