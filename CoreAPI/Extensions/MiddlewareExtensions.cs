using Serilog;
using Shared.Domain.Exceptions;
using Shared.Infrastructure.Middlewares;

namespace CoreAPI.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<RolesMiddleware>();
        app.UseMiddleware<TenantMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>(Log.Logger);
        app.UseMiddleware<ValidationExceptionMiddleware>(Log.Logger);
        app.UseMiddleware<WebExceptionHandlingMiddleware>(Log.Logger);

        app.Use(
            async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (DebugException ex)
                {
                    Console.WriteLine(ex);
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(ex.ToString());
                }
            }
        );

        return app;
    }
}
