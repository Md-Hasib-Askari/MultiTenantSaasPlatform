using System.Numerics;
using System.Security.Cryptography;
using Api.Authorization;
using Api.Middleware;
using Application.Auth;
using Application.Common;
using Application.Projects.Interfaces;
using Application.Projects.Services;
using Application.Tasks.Interfaces;
using Application.Tasks.Services;
using Application.Tenants.Interfaces;
using Application.Tenants.Services;
using Application.Users.Interfaces;
using Application.Users.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Projects;
using Infrastructure.Seed;
using Infrastructure.Tasks;
using Infrastructure.Tenants;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    )
    .AddPolicy(
        "ProjectMemberView",
        policy => policy.Requirements.Add(new ProjectMemberRoleRequirement("Member", "Editor", "Admin"))
    )
    .AddPolicy(
        "ProjectMemberAdmin",
        policy => policy.Requirements.Add(new ProjectMemberRoleRequirement("Admin"))
    )
    .AddPolicy(
        "ProjectMemberEdit",
        policy => policy.Requirements.Add(new ProjectMemberRoleRequirement("Editor", "Admin"))
    )
    .AddPolicy(
        "TaskUpdate",
        policy => policy.Requirements.Add(new TaskUpdateRequirement())
    );
builder.Services.AddSingleton<IAuthorizationHandler, TenantRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TenantMemberHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProjectMemberRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TaskUpdateHandler>();
builder.Services.AddHttpContextAccessor();

var jwtOpts = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var rsa = RSA.Create(2048);
if (!string.IsNullOrEmpty(jwtOpts.PrivateKey))
    rsa.ImportFromPem(jwtOpts.PrivateKey);

builder.Services.AddSingleton(rsa);

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

        o.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/problem+json";
                var body = new
                {
                    type = "https://httpstatuses.com/401",
                    title = "Unauthorized",
                    status = 401,
                    detail = context.ErrorDescription ?? "Authentication is required.",
                    instance = context.Request.Path,
                    traceId = context.HttpContext.TraceIdentifier,
                };
                return context.Response.WriteAsJsonAsync(body);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/problem+json";
                var body = new
                {
                    type = "https://httpstatuses.com/403",
                    title = "Forbidden",
                    status = 403,
                    detail = "You are not authorized to access this resource.",
                    instance = context.Request.Path,
                    traceId = context.HttpContext.TraceIdentifier,
                };
                return context.Response.WriteAsJsonAsync(body);
            },
        };
    });

// Add services to the container.
builder
    .Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        try
        {
            var cfg = ConfigurationOptions.Parse(redisConnectionString);
            cfg.AbortOnConnectFail = false;
            cfg.ConnectTimeout = 5000;
            cfg.SyncTimeout = 5000;
            return ConnectionMultiplexer.Connect(cfg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis connection failed (degraded mode): {ex.Message}");
            return null!;
        }
    });
}

// Tenant Context (Scoped = one per request)
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

// Cache Service
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}

// Entity Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IUserService, UserService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
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

        var rawConnStr = builder.Configuration.GetConnectionString("Postgres")!;
        if (rawConnStr.StartsWith("postgresql://"))
        {
            var uri = new Uri(rawConnStr);
            var parts = uri.UserInfo.Split(':');
            var host = uri.Host;
            var port = uri.Port;
            var db = uri.AbsolutePath.TrimStart('/');
            rawConnStr = $"Host={host};Port={port};Database={db};Username={parts[0]};Password={parts[1]};SSL Mode=Require";
        }

        options
            .UseNpgsql(rawConnStr)
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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();

// SSL is terminated at the load balancer (DO App Platform).
// Internal health checks use HTTP, so redirect would break them.
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Auto-apply pending EF Core migrations on startup (safe for deployments)
using (var migrateScope = app.Services.CreateScope())
{
    var db = migrateScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (args.Contains("--seed"))
{
    using var seedScope = app.Services.CreateScope();
    var seeder = seedScope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
    return;
}

app.Run();
