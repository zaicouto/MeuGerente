using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOs;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Middleware que captura exceções de validação e formata a resposta de erro.
/// </summary>
public class ValidationExceptionMiddleware(
    RequestDelegate next,
    Serilog.ILogger logger
)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.Information(
                "ValidationException: {Errors} | Path={Path} | Method={Method}",
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                context.Request.Path,
                context.Request.Method
            );
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            Dictionary<string, string[]> errors = ex
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            var response = new { Errors = errors };
            await context.Response.WriteAsJsonAsync(
                new ApiResponse(false, "A validação falhou.", response)
            );
        }
        catch (Exception)
        {
            throw;
        }
    }
}
