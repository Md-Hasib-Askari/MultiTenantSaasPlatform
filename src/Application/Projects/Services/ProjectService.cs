using Application.Projects.DTOs;
using Application.Projects.Interfaces;
using Domain.Entities.Projects;

namespace Application.Projects.Services;

public class ProjectService(IProjectRepository projectRepo) : IProjectService
{
    private readonly IProjectRepository _projectRepo = projectRepo;

    public async Task AddAsync(
        Guid tenantId,
        CreateProjectRequest createProjectDto,
        Guid createdById,
        CancellationToken ct = default
    )
    {
        await _projectRepo.AddAsync(tenantId, createProjectDto, createdById, ct);
    }

    public async Task<IReadOnlyList<Project>> GetAllByTenantIdAsync(
        Guid tenantId,
        CancellationToken ct = default
    )
    {
        return await _projectRepo.GetAllByTenantIdAsync(tenantId, ct);
    }

    public async Task<Project> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _projectRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Project with ID {id} not found.");
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
        UpdateProjectRequest updateProjectDto,
        CancellationToken ct = default
    )
    {
        await _projectRepo.UpdateAsync(tenantId, projectId, updateProjectDto, ct);
    }
}
