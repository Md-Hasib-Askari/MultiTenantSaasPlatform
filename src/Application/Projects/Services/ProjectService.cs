using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Entities.Projects;

namespace Application.Projects.Services;

public class ProjectService(IProjectRepository projectRepo) : IProjectService
{
    private readonly IProjectRepository _projectRepo = projectRepo;

    public async Task AddAsync(
        Guid tenantId,
        CreateProjectRequest request,
        Guid createdById,
        CancellationToken ct = default
    )
    {
        await _projectRepo.AddAsync(tenantId, request, createdById, ct);
    }

    public async Task<IReadOnlyList<ProjectResponse>> GetAllByTenantIdAsync(
        Guid tenantId,
        CancellationToken ct = default
    )
    {
        var projects = await _projectRepo.GetAllByTenantIdAsync(tenantId, ct);
        return projects.Select(MapToResponse).ToList();
    }

    public async Task<ProjectResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _projectRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Project with ID {id} not found.");
        return MapToResponse(project);
    }

    public async Task<bool> ExistsAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken ct = default
    )
    {
        return await _projectRepo.ExistsAsync(tenantId, projectId, ct);
    }

    public Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken ct = default)
    {
        return _projectRepo.NameExistsAsync(tenantId, name, ct);
    }

    public async Task UpdateAsync(
        Guid tenantId,
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken ct = default
    )
    {
        await _projectRepo.UpdateAsync(tenantId, projectId, request, ct);
    }

    private static ProjectResponse MapToResponse(Project p) => new(
        p.Id,
        p.TenantId,
        p.Name,
        p.Description,
        p.Color,
        p.IsActive,
        p.CreatedAt,
        p.CreatedById,
        p.UpdatedAt,
        p.UpdatedById
    );
}
