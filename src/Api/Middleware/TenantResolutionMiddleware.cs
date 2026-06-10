using Domain.Enums;
using Domain.Interfaces;
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
        {
            // fetch by slud
        }

        // Strategy 2: JWT clain (tenant_id claim in Bearer token)
        if (tenant is null)
        {
            var claim = ctx.User.FindFirst("tenant_id").Value;
            if (Guid.TryParse(claim, out var tid))
            {
                // fetch by id
            }
        }

        // Strategy 3: API key header (X-API-Key)
        if (tenant is null)
        {
            var key = ctx.Request.Headers["X-API-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
            {
                // try by api key
            }
        }

        // Strategy 4: Query param (dev only) eg. /?tenant=acme
        if (tenant is null && _env.IsDevelopment())
        {
            var slug = ctx.Request.Query["tenant"].FirstOrDefault();
            if (!string.IsNullOrEmpty(slug))
            {
                // fetch by slug
            }
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
}
