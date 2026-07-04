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
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Projects;
using Infrastructure.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ASP.NET Identity
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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

// Entity Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Respositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

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

app.Run();
