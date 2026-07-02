using Application.Projects.Interfaces;
using Domain.Entities.Projects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Projects;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Project project, CancellationToken ct = default)
    {
        // await Project.Create(Project project);
        // TODO: get the tenantId from the project and check if it exists in the database
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
