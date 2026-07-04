using System.Text.Json;
using Application.Common;
using Domain.Entities;
using StackExchange.Redis;

namespace Infrastructure.Tenants;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task InvalidateTenantAsync(
        string slug,
        Guid tenantId,
        CancellationToken ct = default
    )
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync($"tenant:{slug}");
        await db.KeyDeleteAsync($"tenant:id:{tenantId}");
    }

    public async Task SetTenantInfoAsync(Tenant tenant, CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var info = new TenantInfo(
            tenant.Id,
            tenant.Slug,
            tenant.Name,
            tenant.Plan,
            tenant.Status,
            tenant.DeletedAt
        );
        var json = JsonSerializer.Serialize(info);
        await db.StringSetAsync($"tenant:{tenant.Slug}", json, TimeSpan.FromMinutes(5));
        await db.StringSetAsync($"tenant:id:{tenant.Id}", json, TimeSpan.FromMinutes(5));
    }
}
