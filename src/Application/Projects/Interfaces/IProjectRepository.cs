using Domain.Entities.Projects;

namespace Application.Projects.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken ct = default);
}
