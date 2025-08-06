using Serilog;
using Shared.Domain.Exceptions;
using Shared.Infrastructure.Middlewares;

namespace CoreAPI.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        // Middlewares para autenticação e autorização
        app.UseMiddleware<RolesMiddleware>();
        app.UseMiddleware<TenantMiddleware>();

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
                    Console.WriteLine(ex);
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(ex.ToString());
                }
            }
        );

        return app;
    }
}
