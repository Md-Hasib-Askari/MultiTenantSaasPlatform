using Application.Projects.DTOs;
using Domain.Entities.Projects;

namespace Application.Projects.Interfaces;

public interface IProjectMemberService
{
    Task<ProjectMemberResponse?> GetMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<ProjectMemberResponse>> GetMembersByProjectIdAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct = default
    );
    Task<bool> IsMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    );
    Task AddMemberAsync(Guid tenantId, Guid projectId, Guid userId, Guid addedById, CancellationToken ct = default);
    Task RemoveMemberAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        Guid deletedBy,
        CancellationToken ct = default
    );
    Task UpdateMemberRoleAsync(
        Guid tenantId,
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    );
}
