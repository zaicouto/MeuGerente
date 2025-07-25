using FluentValidation;
using Serilog;

namespace WebAPI.Infrastructure.Middlewares;

public class ValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            Log.Warning(
                "ValidationException: {Errors} | Path={Path} | Method={Method}",
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                context.Request.Path,
                context.Request.Method
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var response = new { Errors = errors };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Unhandled exception at {Path} | Method={Method}",
                context.Request.Path,
                context.Request.Method
            );

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Error = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
