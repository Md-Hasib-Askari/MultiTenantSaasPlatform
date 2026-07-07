using Domain.Entities.Projects;

namespace Application.Projects.Interfaces;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<ProjectMember>> GetMembersByProjectIdAsync(
        Guid projectId,
        CancellationToken ct = default
    );
    Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
    Task AddMemberAsync(
        Guid projectId,
        Guid userId,
        Guid createdById,
        CancellationToken ct = default
    );
    Task RemoveMemberAsync(
        Guid projectId,
        Guid userId,
        Guid deletedBy,
        CancellationToken ct = default
    );
    Task UpdateMemberRoleAsync(
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    );
}
