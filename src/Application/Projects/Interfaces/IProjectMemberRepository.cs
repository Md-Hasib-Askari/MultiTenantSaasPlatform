using Domain.Entities.Projects;

namespace Application.Projects.Interfaces;

public interface IProjectMemberRepository
{
    Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
    Task AddMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
    Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
    Task UpdateMemberRoleAsync(
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    );
}
