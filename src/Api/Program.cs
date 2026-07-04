using System.Numerics;
using System.Security.Cryptography;
using Api.Authorization;
using Api.Middleware;
using Application.Auth;
using Application.Common;
using Application.Projects.Interfaces;
using Application.Projects.Services;
using Application.Tenants.Interfaces;
using Application.Tenants.Services;
using Application.Users.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Projects;
using Infrastructure.Seed;
using Infrastructure.Tenants;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Password hasher (replaces Identity's built-in registration)
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

// JWT Authentication
builder.Services.AddOptions<JwtOptions>().Bind(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ApiKeyService>();

// Authorization Policies
builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(
        "RequireTenantOwner",
        policy => policy.Requirements.Add(new TenantRoleRequirement("Owner"))
    )
    .AddPolicy(
        "RequireTenantAdmin",
        policy => policy.Requirements.Add(new TenantRoleRequirement("Owner", "Admin"))
    )
    .AddPolicy(
        "ResourceTenantMember",
        policy => policy.Requirements.Add(new TenantMemberRequirement("Owner"))
    )
    .AddPolicy(
        "ResourceTenantAdmin",
        policy => policy.Requirements.Add(new TenantMemberRequirement("Owner", "Member"))
    );
builder.Services.AddSingleton<IAuthorizationHandler, TenantRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TenantMemberHandler>();

var jwtOpts = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var rsa = RSA.Create(2048);
if (!string.IsNullOrEmpty(jwtOpts.PrivateKey))
    rsa.ImportFromPem(jwtOpts.PrivateKey);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOpts.Issuer,
            ValidAudience = jwtOpts.Audience,
            IssuerSigningKey = new RsaSecurityKey(rsa),
        };
    });

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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

// Entity Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserTenantRoleRepository, UserTenantRoleRepository>();

// Seeder
builder.Services.AddScoped<DataSeeder>();

// Database Configuration
// builder.Services.AddSingleton<ITenantContext, StubTenantContext>();
builder.Services.AddDbContext<AppDbContext>(
    (sp, options) =>
    {
        var tenantContext = sp.GetRequiredService<ITenantContext>();

        options
            .UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
            // .UseSnakeCaseNamingConvention()
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
    return;
}

app.Run();
