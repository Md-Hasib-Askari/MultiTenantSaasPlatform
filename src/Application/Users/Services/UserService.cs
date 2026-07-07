using Application.Users.DTOs;
using Application.Users.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Users.Services;

public class UserService(IUserRepository repo) : IUserService
{
    private readonly IUserRepository _repo = repo;

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        return Map(user);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest dto, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        user.UpdateProfile(dto.DisplayName, dto.Email);
        await _repo.UpdateAsync(user, ct);

        return Map(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        user.MarkAsDeleted();
        await _repo.UpdateAsync(user, ct);
    }

    public async Task<List<UserResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        var users = await _repo.GetByTenantIdAsync(tenantId, ct);
        return users.Select(Map).ToList();
    }

    private static UserResponse Map(ApplicationUser user) =>
        new(
            user.Id,
            user.Email,
            user.UserName,
            user.DisplayName,
            user.Status,
            user.CreatedAt,
            user.UpdatedAt
        );
}
