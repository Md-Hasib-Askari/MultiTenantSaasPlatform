using System.Security.Claims;
using Domain.Entities.Projects;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Api.Authorization;

public class TaskUpdateHandler(
    AppDbContext db,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<TaskUpdateRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaskUpdateRequirement requirement
    )
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail(new AuthorizationFailureReason(this, "Missing or invalid user claim."));
            return;
        }

        var taskId = ResolveTaskId(httpContextAccessor.HttpContext);
        if (taskId is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Could not resolve task ID from request."));
            return;
        }

        var task = await db
            .TaskItems.AsNoTracking()
            .Where(t => t.Id == taskId.Value)
            .Select(t => new { t.ProjectId, t.AssigneeId })
            .FirstOrDefaultAsync();

        if (task is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Task not found."));
            return;
        }

        // Allow if user is the assignee
        if (task.AssigneeId == userId)
        {
            context.Succeed(requirement);
            return;
        }

        // Allow if user has Editor or Admin role in the project
        var membership = await db
            .ProjectMembers.AsNoTracking()
            .FirstOrDefaultAsync(pm => pm.ProjectId == task.ProjectId && pm.UserId == userId);

        if (membership is not null &&
            (membership.Role is ProjectMemberRole.Admin or ProjectMemberRole.Editor))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(
            new AuthorizationFailureReason(
                this,
                "You are not authorized to update this task. Only the assignee, project editors, and admins can update tasks."
            )
        );
    }

    private static Guid? ResolveTaskId(HttpContext? ctx)
    {
        if (ctx is null)
            return null;

        var routeData = ctx.Request.RouteValues;
        if (routeData.TryGetValue("controller", out var ctrl) &&
            string.Equals(ctrl?.ToString(), "Task", StringComparison.OrdinalIgnoreCase) &&
            routeData.TryGetValue("id", out var idVal) &&
            Guid.TryParse(idVal?.ToString(), out var id))
            return id;

        return null;
    }
}
