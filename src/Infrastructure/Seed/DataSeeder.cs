using Application.Users.Interfaces;
using Domain.Entities;
using Domain.Entities.Projects;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed;

public class DataSeeder(
    AppDbContext db,
    IUserRepository userRepo,
    IPasswordHasher<ApplicationUser> passwordHasher
)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        var user = await SeedUserAsync(ct);
        var tenant = await SeedTenantAsync(user, ct);
        await SeedProjectAsync(tenant, user, ct);
        await UpdateUserTenantAsync(user, tenant, ct);
    }

    private async Task<ApplicationUser> SeedUserAsync(CancellationToken ct)
    {
        var existing = await userRepo.GetByEmailAsync("admin@mtsp.dev", ct);
        if (existing is not null)
            return existing;

        var user = ApplicationUser.Create(
            userName: "admin@mtsp.dev",
            email: "admin@mtsp.dev",
            displayName: "Admin User",
            primaryTenantId: Guid.Empty
        );

        user.SetPasswordHash(passwordHasher.HashPassword(user, "Admin123!"));
        await userRepo.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);

        return user;
    }

    private async Task<Tenant> SeedTenantAsync(ApplicationUser creator, CancellationToken ct)
    {
        var existing = await db.Tenants.FirstOrDefaultAsync(t => t.Slug == "acme", ct);
        if (existing is not null)
            return existing;

        var tenant = Tenant.Create("acme", "Acme Corp", PlanTier.Pro);
        db.Entry(tenant).Property(t => t.CreatedById).CurrentValue = creator.Id;
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync(ct);

        var role = UserTenantRole.Create(creator.Id, tenant.Id, TenantRole.Owner);
        db.UserTenantRoles.Add(role);
        await db.SaveChangesAsync(ct);

        return tenant;
    }

    private async Task SeedProjectAsync(Tenant tenant, ApplicationUser creator, CancellationToken ct)
    {
        if (await db.Projects.AnyAsync(ct))
            return;

        var project = Project.Create(tenant.Id, "Getting Started", "Onboarding guide", "#63B3ED");
        db.Entry(project).Property(p => p.CreatedById).CurrentValue = creator.Id;
        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);
    }

    private async Task UpdateUserTenantAsync(ApplicationUser user, Tenant tenant, CancellationToken ct)
    {
        if (user.PrimaryTenantId == tenant.Id)
            return;

        user.SetPrimaryTenantId(tenant.Id);
        await db.SaveChangesAsync(ct);
    }
}
