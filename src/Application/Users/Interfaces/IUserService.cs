using Application.Users.DTOs;

namespace Application.Users.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<UserResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default);
}
