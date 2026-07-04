using Application.Tenants.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tenants;

public class TenantRepository(AppDbContext db) : ITenantRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default) =>
        await _db.Tenants.AddAsync(tenant, ct);

    public Task<Tenant?> GetByApiKeyHashAsync(string hash, CancellationToken ct = default) =>
        _db
            .ApiKeys.AsNoTracking()
            .Where(k => k.KeyHash == hash && k.IsActive && k.DeletedAt == null)
            .Where(k => k.ExpiresAt != null || k.ExpiresAt > DateTimeOffset.UtcNow)
            .Select(k => k.Tenant)
            .FirstOrDefaultAsync(ct);

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Tenants
            .FirstOrDefaultAsync(
                t => t.Id == id && t.Status != TenantStatus.Deleted && t.DeletedAt == null,
                ct
            );

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.Slug == slug && t.Status != TenantStatus.Deleted && t.DeletedAt == null,
                ct
            );

    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        _db.Tenants.AnyAsync(t => t.Slug == slug && t.DeletedAt == null, ct);

    public async Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        _db.Tenants.Update(tenant);
        await _db.SaveChangesAsync(ct);
    }
}
