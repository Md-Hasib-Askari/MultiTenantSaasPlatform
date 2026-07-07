using Application.Tenants.DTOs;
using Domain.Entities;

namespace Application.Tenants.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tenant?> GetByApiKeyHashAsync(string hash, CancellationToken ct = default);
    Task AddAsync(CreateTenantRequest createTenantDto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateTenantRequest updateTenantDto, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
}
