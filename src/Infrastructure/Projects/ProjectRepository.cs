using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Entities.Projects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Projects;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(
        Guid tenantId,
        CreateProjectRequest createProjectDto,
        Guid createdById,
        CancellationToken ct = default
    )
    {
        var newProject = Project.Create(
            tenantId,
            createProjectDto.Name,
            createProjectDto.Description,
            createProjectDto.Color,
            createdById
        );
        await _context.Projects.AddAsync(newProject, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Project>> GetAllByTenantIdAsync(
        Guid tenantId,
        CancellationToken ct = default
    )
    {
        return await _context
            .Projects.Where(p => p.TenantId == tenantId)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .ToListAsync(ct);
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var project = _context
            .Projects.Include(p => p.Members)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return project;
    }

    public Task<bool> ExistsAsync(Guid tenantId, Guid projectId, CancellationToken ct = default)
    {
        return _context.Projects.AnyAsync(p => p.TenantId == tenantId && p.Id == projectId, ct);
    }

    public Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken ct = default)
    {
        return _context.Projects.AnyAsync(p => p.TenantId == tenantId && p.Name == name, ct);
    }

    public async Task UpdateAsync(
        Guid tenantId,
        Guid projectId,
        UpdateProjectRequest updateProjectDto,
        CancellationToken ct = default
    )
    {
        var project =
            _context
                .Projects.Where(p => p.Id == projectId && p.TenantId == tenantId)
                .FirstOrDefault()
            ?? throw new InvalidOperationException("Project not found.");

        project.UpdateDetails(
            updateProjectDto.Name,
            updateProjectDto.Description,
            updateProjectDto.Color
        );

        await _context.SaveChangesAsync(ct);
    }
}
