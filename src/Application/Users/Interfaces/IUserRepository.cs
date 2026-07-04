using Domain.Entities;

namespace Application.Users.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ApplicationUser user, CancellationToken ct = default);
}
