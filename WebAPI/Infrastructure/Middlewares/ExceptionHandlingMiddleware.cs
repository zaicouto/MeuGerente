using Shared.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace WebAPI.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Aconteceu uma exceção não tratada.");
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

        var problemDetails = new
        {
            type = $"https://httpstatuses.io/{(int)status}",
            title,
            status = (int)status,
            detail = exception.Message,
            instance = context.Request.Path,
        };

        string payload = JsonSerializer.Serialize(problemDetails);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(payload);
    }
}
