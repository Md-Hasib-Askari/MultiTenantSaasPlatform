using Domain.Entities;

namespace Domain.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetBySlugAsyc(string slug, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tenant?> GetByApiKeyHashAsync(string hash, CancellationToken ct = default);
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
}
