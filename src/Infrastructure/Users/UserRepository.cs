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

    public async Task AddAsync(ApplicationUser user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
    }
}
