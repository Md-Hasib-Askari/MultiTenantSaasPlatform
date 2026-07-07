using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Entities.Projects;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/members")]
public class ProjectMemberController(
    IProjectMemberService projectMemberService,
    ITenantContext tenantContext
) : ControllerBase
{
    [Authorize(Policy = "ProjectMemberView")]
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken ct)
    {
        var members = await projectMemberService.GetMembersByProjectIdAsync(
            tenantContext.TenantId, projectId, ct);
        return Ok(members.Select(MapToDto));
    }

    [Authorize(Policy = "ProjectMemberView")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMember(Guid projectId, Guid userId, CancellationToken ct)
    {
        var member = await projectMemberService.GetMemberAsync(
            tenantContext.TenantId, projectId, userId, ct);

        if (member is null)
            return NotFound(new { error = "Member not found." });

        return Ok(MapToDto(member));
    }

    [Authorize(Policy = "ProjectMemberAdmin")]
    [HttpPost]
    public async Task<IActionResult> AddMember(
        Guid projectId,
        [FromBody] AddProjectMemberRequest dto,
        CancellationToken ct
    )
    {
        var userId = User.GetUserId();
        await projectMemberService.AddMemberAsync(
            tenantContext.TenantId, projectId, dto.UserId, userId, ct);
        return Ok();
    }

    [Authorize(Policy = "ProjectMemberAdmin")]
    [HttpPatch("{userId}")]
    public async Task<IActionResult> UpdateRole(
        Guid projectId,
        Guid userId,
        [FromBody] UpdateProjectMemberRoleRequest dto,
        CancellationToken ct
    )
    {
        await projectMemberService.UpdateMemberRoleAsync(
            tenantContext.TenantId, projectId, userId, dto.Role, ct);
        return Ok();
    }

    [Authorize(Policy = "ProjectMemberAdmin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid userId,
        CancellationToken ct
    )
    {
        await projectMemberService.RemoveMemberAsync(
            tenantContext.TenantId, projectId, userId, ct);
        return Ok();
    }

    private static ProjectMemberResponse MapToDto(ProjectMember m) => new(m.UserId, m.Role);
}
