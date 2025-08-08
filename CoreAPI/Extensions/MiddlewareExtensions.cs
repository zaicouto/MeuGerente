using Serilog;
using Shared.Application.DTOs;
using Shared.Domain.Exceptions;
using Shared.Infrastructure.Middlewares;

namespace CoreAPI.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(
        this IApplicationBuilder app
    )
    {
        // Middlewares para tratamento de exceções e logging
        app.UseMiddleware<RequestLoggingMiddleware>(Log.Logger);
        app.UseMiddleware<ValidationExceptionMiddleware>(Log.Logger);
        app.UseMiddleware<WebExceptionHandlingMiddleware>(Log.Logger);

        // Middleware para capturar exceções de debug e retornar detalhes no response
        app.Use(
            async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (DebugException ex)
                {
                    Log.Debug(ex.Message);
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsJsonAsync(new ApiResponse(true, "Debug", ex));
                }
            }
        );
        return app;
    }
}
