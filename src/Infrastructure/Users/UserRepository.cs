using Application.Users.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public Task<List<ApplicationUser>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        return context
            .UserTenantRoles.Where(r => r.TenantId == tenantId)
            .Select(r => r.User)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ApplicationUser user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
    }

    public async Task UpdateAsync(ApplicationUser user, CancellationToken ct = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }
}
