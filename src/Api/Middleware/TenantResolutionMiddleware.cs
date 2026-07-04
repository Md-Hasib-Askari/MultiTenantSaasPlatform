using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Tenants.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Tenants;
using StackExchange.Redis;

namespace Api.Middleware;

public class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> log,
    IWebHostEnvironment env
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<TenantResolutionMiddleware> _log = log;
    private readonly IWebHostEnvironment _env = env;

    public async Task InvokeAsync(
        HttpContext ctx,
        TenantContext tenantCtx,
        ITenantRepository tenantRepo,
        IConnectionMultiplexer redis
    )
    {
        var path = ctx.Request.Path.Value ?? "";
        if (path.StartsWith("/health") || path.StartsWith("/metrics"))
        {
            await _next(ctx);
            return;
        }

        TenantInfo? tenant = null;

        // Strategy 1: Subdomain (acme.app.example.com)
        var host = ctx.Request.Host.Host;
        var parts = host.Split('.');
        if (parts.Length >= 3 && parts[0] != "www")
            tenant = await TryBySlugAsync(parts[0], tenantRepo, redis);

        // Strategy 2: JWT clain (tenant_id claim in Bearer token)
        if (tenant is null)
        {
            var claim = ctx.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(claim, out var tid))
                tenant = await TryByIdAsync(tid, tenantRepo, redis);
        }

        // Strategy 3: API key header (X-API-Key)
        if (tenant is null)
        {
            var key = ctx.Request.Headers["X-API-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                tenant = await TryByApiKeyAsync(key, tenantRepo);
        }

        // Strategy 4: Query param (dev only) eg. /?tenant=acme
        if (tenant is null && _env.IsDevelopment())
        {
            var slug = ctx.Request.Query["tenant"].FirstOrDefault();
            if (!string.IsNullOrEmpty(slug))
                tenant = await TryBySlugAsync(slug, tenantRepo, redis);
        }

        if (tenant is null)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant not identified", requestId = ctx.TraceIdentifier }
            );
            return;
        }

        if (tenant.Status == TenantStatus.Suspended)
        {
            ctx.Response.StatusCode = 403;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Account suspended. Contact Support." }
            );
            return;
        }

        if (tenant.Status == TenantStatus.Deleted || tenant.DeletedAt != null)
        {
            ctx.Response.StatusCode = 404;
            return;
        }

        tenantCtx.SetTenant(tenant);
        ctx.Response.Headers["X-Tenant-Id"] = tenant.Id.ToString();
        ctx.Response.Headers["X-Tenant-Plan"] = tenant.Plan.ToString();

        await _next(ctx);
    }

    private static async Task<TenantInfo?> TryBySlugAsync(
        string slug,
        ITenantRepository tenantRepo,
        IConnectionMultiplexer redis
    )
    {
        var db = redis.GetDatabase();
        var key = $"tenant:{slug}";
        var hit = await db.StringGetAsync(key);

        if (hit.HasValue)
            return JsonSerializer.Deserialize<TenantInfo>(hit.ToString());

        var tenant = await tenantRepo.GetBySlugAsync(slug);
        if (tenant is null)
            return null;

        var info = Map(tenant);
        var json = JsonSerializer.Serialize(info);
        await db.StringSetAsync($"tenant:{tenant.Slug}", json, TimeSpan.FromMinutes(5));
        await db.StringSetAsync($"tenant:it:{tenant.Id}", json, TimeSpan.FromMinutes(5));
        return info;
    }

    private static async Task<TenantInfo?> TryByIdAsync(
        Guid id,
        ITenantRepository repo,
        IConnectionMultiplexer redis
    )
    {
        var db = redis.GetDatabase();
        var key = $"tenant:id:{id}";
        var hit = await db.StringGetAsync(key);

        if (hit.HasValue)
            return JsonSerializer.Deserialize<TenantInfo>(hit.ToString());

        var tenant = await repo.GetByIdAsync(id);
        if (tenant is null)
            return null;

        var info = Map(tenant);
        var json = JsonSerializer.Serialize(info);
        await db.StringSetAsync(key, json, TimeSpan.FromMinutes(5));
        return info;
    }

    private static async Task<TenantInfo?> TryByApiKeyAsync(string rawKey, ITenantRepository repo)
    {
        var pepper = Convert.FromBase64String(
            Environment.GetEnvironmentVariable("API_KEY_PEPPER")
                ?? "QXBpS2V5UGVwcGVyU2VlZEZvckRldg=="
        );
        using var hmac = new HMACSHA256(pepper);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawKey)));
        var tenant = await repo.GetByApiKeyHashAsync(hash);
        return tenant is null ? null : Map(tenant);
    }

    private static TenantInfo Map(Tenant t) =>
        new(t.Id, t.Slug, t.Name, t.Plan, t.Status);
}
