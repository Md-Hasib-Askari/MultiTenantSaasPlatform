using Application.Projects.Interfaces;
using Domain.Entities.Projects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Projects;

public class ProjectMemberRepository(AppDbContext db) : IProjectMemberRepository
{
    private readonly AppDbContext _db = db;

    public async Task<bool> IsMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        return await _db.ProjectMembers.AnyAsync(
            pm => pm.ProjectId == projectId && pm.UserId == userId,
            ct
        );
    }

    public async Task AddMemberAsync(
        Guid projectId,
        Guid userId,
        Guid createdById,
        CancellationToken ct = default
    )
    {
        var project =
            await _db
                .Projects.Where(p => p.Id == projectId)
                .Select(p => new { p.TenantId })
                .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("Project not found.");

        var projectMember = ProjectMember.Create(project.TenantId, projectId, userId, createdById);

        _db.ProjectMembers.Add(projectMember);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(
        Guid projectId,
        Guid userId,
        Guid deletedBy,
        CancellationToken ct = default
    )
    {
        var projectMember =
            await _db
                .ProjectMembers.Where(pm => pm.ProjectId == projectId && pm.UserId == userId)
                .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("Project member not found.");

        projectMember.MarkAsDeleted(deletedBy);
        _db.ProjectMembers.Update(projectMember);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateMemberRoleAsync(
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    )
    {
        var projectMember =
            await _db.ProjectMembers.FirstOrDefaultAsync(
                pm => pm.ProjectId == projectId && pm.UserId == userId,
                ct
            ) ?? throw new InvalidOperationException("Project member not found.");

        projectMember.ChangeRole(newRole);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<ProjectMember?> GetMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        return await _db.ProjectMembers.FirstOrDefaultAsync(
            pm => pm.ProjectId == projectId && pm.UserId == userId,
            ct
        );
    }

    public async Task<IReadOnlyList<ProjectMember>> GetMembersByProjectIdAsync(
        Guid projectId,
        CancellationToken ct = default
    )
    {
        return await _db.ProjectMembers.Where(pm => pm.ProjectId == projectId).ToListAsync(ct);
    }
}
