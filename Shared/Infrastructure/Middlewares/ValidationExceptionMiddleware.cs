using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
            logger.Warning(
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
        catch (Exception ex)
        {
            logger.Error(
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
            await context.Response.WriteAsJsonAsync(
                new ApiResponse(false, "Um erro inesperado aconteceu.", response)
            );
        }
    }
}
