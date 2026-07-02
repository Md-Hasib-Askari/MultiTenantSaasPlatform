using Application.Projects.Interfaces;
using Domain.Entities.Projects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Projects;

public class ProjectMemberRepository(AppDbContext context) : IProjectMemberRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> IsMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        return await _context.ProjectMembers.AnyAsync(
            pm => pm.ProjectId == projectId && pm.UserId == userId,
            ct
        );
    }

    public async Task AddMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default)
    {
        var projectMember = ProjectMember.Create(projectId, userId);

        _context.ProjectMembers.Add(projectMember);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default)
    {
        var projectMember = await _context
            .ProjectMembers.Where(pm => pm.ProjectId == projectId && pm.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (projectMember != null)
        {
            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateMemberRoleAsync(
        Guid projectId,
        Guid userId,
        ProjectMemberRole newRole,
        CancellationToken ct = default
    )
    {
        var projectMember = await _context.ProjectMembers.FirstOrDefaultAsync(
            pm => pm.ProjectId == projectId && pm.UserId == userId,
            ct
        );

        if (projectMember != null)
        {
            ProjectMember.ChangeRole(projectMember, newRole);
            await _context.SaveChangesAsync(ct);
        }
    }
}
