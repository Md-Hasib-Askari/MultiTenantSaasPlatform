using System.Globalization;
using System.Security.Claims;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Authorization;

/// <summary>
/// Resource-based handler that validates the current user is a member of
/// the target tenant (via UserTenantRole join table) and holds the required
/// role for that tenant.
/// </summary>
public class TenantMemberHandler(AppDbContext db) : AuthorizationHandler<TenantMemberRequirement>
{
    private readonly AppDbContext _db = db;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantMemberRequirement requirement
    )
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenantIdClaim = context.User.FindFirstValue("tenant_id");

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(tenantIdClaim))
        {
            context.Fail(new AuthorizationFailureReason(this, "Missing user or tenant claims."));
            return;
        }

        if (
            !Guid.TryParse(userIdClaim, out var userId)
            || !Guid.TryParse(tenantIdClaim, out var tenantId)
        )
        {
            context.Fail(
                new AuthorizationFailureReason(this, "Invalid user or tenant claim format.")
            );
            return;
        }

        var membership = await _db
            .UserTenantRoles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TenantId == tenantId);
        if (membership is null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, "User is not a member of this tenant.")
            );
            return;
        }

        if (requirement.AllowedRoles.Contains(membership.Role, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(
                new AuthorizationFailureReason(
                    this,
                    $"Tenant role '{membership.Role}' is not authorized. Required: {string.Join(", ", requirement.AllowedRoles)}."
                )
            );
        }
    }
}
