using System.Text.Json;
using Application.Tenants.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Tenants;
using StackExchange.Redis;

namespace Api.Middleware;

public class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> log
)
{
    private const string TenantIdHeader = "X-Tenant-Id";
    private readonly RequestDelegate _next = next;
    private readonly ILogger<TenantResolutionMiddleware> _log = log;

    public async Task InvokeAsync(
        HttpContext ctx,
        TenantContext tenantCtx,
        ITenantRepository tenantRepo,
        IConnectionMultiplexer redis
    )
    {
        var path = "/" + (ctx.Request.Path.Value ?? "").TrimStart('/');
        _log.LogDebug(
            "TenantResolutionMiddleware path={Path} method={Method}",
            path,
            ctx.Request.Method
        );

        if (
            path.StartsWith("/health")
            || path.StartsWith("/metrics")
            || path.StartsWith("/openapi")
            || path.StartsWith("/api/auth")
            || (path == "/api/tenants" && ctx.Request.Method == "POST")
        )
        {
            _log.LogTrace("Bypassing tenant resolution for {Path}", path);
            await _next(ctx);
            return;
        }

        _log.LogDebug("Checking tenant header for {Path}", path);
        var headerValue = ctx.Request.Headers[TenantIdHeader].FirstOrDefault();
        _log.LogDebug("X-Tenant-Id header value: {Header}", headerValue);
        if (headerValue is null || !Guid.TryParse(headerValue, out var tenantId))
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant not identified", requestId = ctx.TraceIdentifier }
            );
            return;
        }

        var tenant = await TryByIdAsync(tenantId, tenantRepo, redis);
        if (tenant is null)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Tenant not found", requestId = ctx.TraceIdentifier }
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

        var info = MapToTenantInfo(tenant);
        var json = JsonSerializer.Serialize(info);
        await db.StringSetAsync(key, json, TimeSpan.FromMinutes(5));
        await db.StringSetAsync($"tenant:{tenant.Slug}", json, TimeSpan.FromMinutes(5));
        return info;
    }

    private static TenantInfo MapToTenantInfo(Tenant t) =>
        new(t.Id, t.Slug, t.Name, t.Plan, t.Status, t.DeletedAt);
}
