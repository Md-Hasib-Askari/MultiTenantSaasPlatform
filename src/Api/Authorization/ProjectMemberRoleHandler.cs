using System.Security.Claims;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Api.Authorization;

public class ProjectMemberRoleHandler(
    AppDbContext db,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<ProjectMemberRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectMemberRoleRequirement requirement
    )
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail(new AuthorizationFailureReason(this, "Missing or invalid user claim."));
            return;
        }

        // Try to resolve project ID from the resource (passed via IAuthorizationService)
        // or from the route data
        var projectId = context.Resource is Guid resourceId
            ? resourceId
            : await ResolveProjectIdAsync(httpContextAccessor.HttpContext);

        if (projectId is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Could not resolve project ID from request."));
            return;
        }

        var membership = await db
            .ProjectMembers.AsNoTracking()
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId.Value && pm.UserId == userId);

        if (membership is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not a member of this project."));
            return;
        }

        var role = membership.Role.ToString();
        if (requirement.AllowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(
                new AuthorizationFailureReason(
                    this,
                    $"Project role '{role}' is not authorized. Required: {string.Join(", ", requirement.AllowedRoles)}."
                )
            );
        }
    }

    private async Task<Guid?> ResolveProjectIdAsync(HttpContext? ctx)
    {
        if (ctx is null)
            return null;

        var routeData = ctx.Request.RouteValues;

        // Check for {projectId} (ProjectMemberController, TaskController by-project)
        if (routeData.TryGetValue("projectId", out var projectIdVal) &&
            Guid.TryParse(projectIdVal?.ToString(), out var pid))
            return pid;

        // Check for {id} on project routes (ProjectController)
        if (routeData.TryGetValue("controller", out var ctrl) &&
            string.Equals(ctrl?.ToString(), "Project", StringComparison.OrdinalIgnoreCase) &&
            routeData.TryGetValue("id", out var idVal) &&
            Guid.TryParse(idVal?.ToString(), out var id))
            return id;

        // Check for {id} on task routes — look up the task to find its project
        if (routeData.TryGetValue("controller", out var taskCtrl) &&
            string.Equals(taskCtrl?.ToString(), "Task", StringComparison.OrdinalIgnoreCase) &&
            routeData.TryGetValue("id", out var taskIdVal) &&
            Guid.TryParse(taskIdVal?.ToString(), out var taskId))
        {
            var taskProject = await db
                .TaskItems.AsNoTracking()
                .Where(t => t.Id == taskId)
                .Select(t => t.ProjectId)
                .FirstOrDefaultAsync();

            if (taskProject != Guid.Empty)
                return taskProject;
        }

        return null;
    }
}
