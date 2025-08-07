using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOs;
using Shared.Domain.Exceptions;
using System.Net;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Middleware que captura exceções de requisição web e formata a resposta de erro.
/// </summary>
public class WebExceptionHandlingMiddleware(
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
        catch (Exception ex)
        {
            if (
                ex
                is NotFoundException
                    or BadRequestException
                    or UnauthorizedException
                    or ForbiddenException
                    or ConflictException
            )
            {
                logger.Warning(ex, "Aconteceu uma exceção de requisição web.");
            }
            else
            {
                logger.Error(ex, "Aconteceu uma exceção não tratada.");
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string title;
        switch (exception)
        {
            case NotFoundException:
                status = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                break;

            case BadRequestException:
                status = HttpStatusCode.BadRequest;
                title = "Bad Request";
                break;

            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                title = "Unauthorized";
                break;

            case ForbiddenException:
                status = HttpStatusCode.Forbidden;
                title = "Forbidden";
                break;

            case ConflictException:
                status = HttpStatusCode.Conflict;
                title = "Conflict";
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                title = "Internal Server Error";
                break;
        }
        ProblemDetails problemDetails = new()
        {
            Type = $"https://httpstatuses.io/{(int)status}",
            Title = title,
            Status = (int)status,
            Detail = exception.Message,
            Instance = context.Request.Path,
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsJsonAsync(
            new ApiResponse(false, "Uma exceção web foi lançada.", problemDetails)
        );
    }
}
