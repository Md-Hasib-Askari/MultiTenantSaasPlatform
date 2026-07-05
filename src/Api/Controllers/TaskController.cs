using System.Security.Claims;
using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Domain.Entities.Tasks;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var task = await taskService.GetByIdAsync(tenantContext.TenantId, id, ct);
        if (task is null)
            return NotFound(new { error = $"Task with ID {id} not found." });

        return Ok(task);
    }

    [HttpGet("by-project/{projectId}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId, CancellationToken ct)
    {
        var tasks = await taskService.GetByProjectIdAsync(tenantContext.TenantId, projectId, ct);
        return Ok(tasks);
    }

    [HttpGet("by-assignee/{assigneeId}")]
    public async Task<IActionResult> GetByAssigneeId(Guid assigneeId, CancellationToken ct)
    {
        var tasks = await taskService.GetByAssigneeIdAsync(tenantContext.TenantId, assigneeId, ct);
        return Ok(tasks);
    }

    [HttpGet("by-reporter/{reporterId}")]
    public async Task<IActionResult> GetByReporterId(Guid reporterId, CancellationToken ct)
    {
        var tasks = await taskService.GetByReporterIdAsync(tenantContext.TenantId, reporterId, ct);
        return Ok(tasks);
    }

    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetByStatus(TaskItemStatus status, CancellationToken ct)
    {
        var tasks = await taskService.GetByStatusAsync(tenantContext.TenantId, status, ct);
        return Ok(tasks);
    }

    [HttpGet("by-priority/{priority}")]
    public async Task<IActionResult> GetByPriority(TaskPriority priority, CancellationToken ct)
    {
        var tasks = await taskService.GetByPriorityAsync(tenantContext.TenantId, priority, ct);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(new { error = "User not authenticated." });

        try
        {
            await taskService.AddAsync(tenantContext.TenantId, Guid.Parse(userId), dto, ct);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, errors = ex.Errors });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(new { error = "User not authenticated." });

        try
        {
            await taskService.UpdateAsync(dto, id, tenantContext.TenantId, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, errors = ex.Errors });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(new { error = "User not authenticated." });

        try
        {
            await taskService.DeleteAsync(tenantContext.TenantId, id, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
