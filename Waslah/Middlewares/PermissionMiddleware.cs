using Waslah.Authentication.Filters;

namespace Waslah.Middlewares;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var requiredPermissions = endpoint.Metadata
            .GetOrderedMetadata<HasPermissionAttribute>()
            .Select(p => p.Policy)
            .ToList();

        if (!requiredPermissions.Any())
        {
            await _next(context); // No permission required
            return;
        }

        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        var userPermissions = context.User.Claims
            .Where(c => c.Type == Permissions.Type)
            .Select(c => c.Value)
            .ToList();

        foreach (var required in requiredPermissions)
        {
            if (!userPermissions.Contains(required))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Forbidden",
                    message = $"Missing permission: {required}"
                });
                return;
            }
        }

        await _next(context); // Permissions passed
    }
}

