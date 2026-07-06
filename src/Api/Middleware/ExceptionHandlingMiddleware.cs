using Domain.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> log
)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (NotFoundException ex)
        {
            log.LogDebug(ex, "Resource not found");
            await WriteProblemAsync(ctx, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            log.LogDebug(ex, "Key not found");
            await WriteProblemAsync(ctx, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            log.LogDebug(ex, "Unauthorized access");
            await WriteProblemAsync(ctx, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (ConflictException ex)
        {
            log.LogDebug(ex, "Conflict");
            await WriteProblemAsync(ctx, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ValidationException ex)
        {
            log.LogDebug(ex, "Validation failed");
            await WriteProblemAsync(ctx, StatusCodes.Status400BadRequest, ex.Message, ex.Errors);
        }
        catch (FeatureNotAvailableException ex)
        {
            log.LogDebug(ex, "Feature not available");
            await WriteProblemAsync(ctx, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (PlanLimitExceedException ex)
        {
            log.LogDebug(ex, "Plan limit exceeded");
            await WriteProblemAsync(ctx, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            log.LogDebug(ex, "Invalid operation");
            await WriteProblemAsync(ctx, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception");
            await WriteProblemAsync(ctx, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext ctx, int status, string message, object? errors = null)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";

        var body = new Dictionary<string, object?>
        {
            ["type"] = $"https://httpstatuses.com/{status}",
            ["title"] = ReasonPhrases[status],
            ["status"] = status,
            ["detail"] = message,
            ["instance"] = ctx.Request.Path,
            ["traceId"] = ctx.TraceIdentifier,
        };

        if (errors is not null)
            body["errors"] = errors;

        await ctx.Response.WriteAsJsonAsync(body);
    }

    private static readonly Dictionary<int, string> ReasonPhrases = new()
    {
        [400] = "Bad Request",
        [401] = "Unauthorized",
        [403] = "Forbidden",
        [404] = "Not Found",
        [409] = "Conflict",
        [500] = "Internal Server Error",
    };
}
