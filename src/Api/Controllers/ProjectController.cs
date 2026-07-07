using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService projectService, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var projects = await projectService.GetAllByTenantIdAsync(tenantContext.TenantId, ct);
        return Ok(projects);
    }

    [Authorize(Policy = "ProjectMemberView")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var project = await projectService.GetByIdAsync(id, ct);
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await projectService.AddAsync(tenantContext.TenantId, dto, userId, ct);
        return Ok();
    }

    [Authorize(Policy = "ProjectMemberAdmin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest dto, CancellationToken ct)
    {
        await projectService.UpdateAsync(tenantContext.TenantId, id, dto, ct);
        return Ok();
    }
}
