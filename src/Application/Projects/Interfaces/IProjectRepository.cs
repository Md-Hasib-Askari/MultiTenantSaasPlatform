using Application.Projects.DTOs;
using Domain.Entities.Projects;

namespace Application.Projects.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Project>> GetAllByTenantIdAsync(
        Guid tenantId,
        CancellationToken ct = default
    );
    Task AddAsync(
        Guid tenantId,
        CreateProjectRequest createProjectDto,
        Guid createdById,
        CancellationToken ct = default
    );
    Task UpdateAsync(
        Guid tenantId,
        Guid projectId,
        UpdateProjectRequest updateProjectDto,
        CancellationToken ct = default
    );
    Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid tenantId, Guid projectId, CancellationToken ct = default);
}
