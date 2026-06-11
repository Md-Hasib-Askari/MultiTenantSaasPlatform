using Api.Middleware;
using Application.Common;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Tenants;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!)
);

// Tenant Context (Scoped = one per request)
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

// Cache Service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Respositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// Database Configuration
// builder.Services.AddSingleton<ITenantContext, StubTenantContext>();
builder.Services.AddDbContext<AppDbContext>(
    (sp, options) =>
    {
        var tenantContext = sp.GetRequiredService<ITenantContext>();

        options
            .UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new RlsConnectionInterceptor(tenantContext));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<TenantResolutionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
