using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Entities.Projects;

namespace Application.Projects.Services;

public class ProjectMemberService(
    IProjectMemberRepository projectMemberRepo,
    IProjectService projectSvc
) : IProjectMemberService
{
    private readonly IProjectMemberRepository _projectMemberRepo = projectMemberRepo;
    private readonly IProjectService _projectSvc = projectSvc;

    public async Task AddMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        Guid addedById,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        await _projectMemberRepo.AddMemberAsync(projectId, userId, addedById, ct);
    }

    public async Task<ProjectMemberResponse?> GetMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        var member = await _projectMemberRepo.GetMemberAsync(projectId, userId, ct);
        return member is not null ? MapToResponse(member) : null;
    }

    public async Task<IReadOnlyList<ProjectMemberResponse>> GetMembersByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        var members = await _projectMemberRepo.GetMembersByProjectIdAsync(projectId, ct);
        return [.. members.Select(MapToResponse)];
    }

    public async Task<bool> IsMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        return await _projectMemberRepo.IsMemberAsync(projectId, userId, ct);
    }

    public async Task RemoveMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        Guid deletedBy,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        await _projectMemberRepo.RemoveMemberAsync(projectId, userId, deletedBy, ct);
    }

    public async Task UpdateMemberRoleAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    )
    {
        if (!await _projectSvc.ExistsAsync(tenantId, projectId, ct))
            throw new InvalidOperationException("Project not found.");

        await _projectMemberRepo.UpdateMemberRoleAsync(projectId, userId, newRole, ct);
    }

    private static ProjectMemberResponse MapToResponse(ProjectMember m) => new(m.UserId, m.Role);
}
