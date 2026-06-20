using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class TenantRoleHandler : AuthorizationHandler<TenantRoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantRoleRequirement requirement
    )
    {
        var tenantRoleClaim = context.User.FindFirstValue("tenant_role");
        var tenantIdClaim = context.User.FindFirstValue("tenant_id");

        if (string.IsNullOrEmpty(tenantRoleClaim) || string.IsNullOrEmpty(tenantIdClaim))
        {
            context.Fail(new AuthorizationFailureReason(this, "Missing tenant context claims."));
            return Task.CompletedTask;
        }

        if (requirement.AllowedRoles.Contains(tenantIdClaim, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(
                new AuthorizationFailureReason(
                    this,
                    $"Role '{tenantIdClaim}' is not authorized. Required: {string.Join(", ", requirement.AllowedRoles)}."
                )
            );
        }

        return Task.CompletedTask;
    }
}
