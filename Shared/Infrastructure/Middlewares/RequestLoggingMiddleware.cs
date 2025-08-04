using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Security.Claims;

namespace Shared.Infrastructure.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        string method = context.Request.Method;
        PathString path = context.Request.Path;
        string? ip = context.Connection.RemoteIpAddress?.ToString();

        string correlationId =
            context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        string userId =
            context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        logger.Information(
            "Request started: {Method} {Path} from {IP} | CorrelationId={CorrelationId} | UserId={UserId}",
            method,
            path,
            ip,
            correlationId,
            userId
        );

        await next(context);

        stopwatch.Stop();

        int statusCode = context.Response.StatusCode;
        long elapsedMs = stopwatch.ElapsedMilliseconds;

        logger.Information(
            "Request finished: {Method} {Path} → {StatusCode} in {ElapsedMs}ms | CorrelationId={CorrelationId} | UserId={UserId}",
            method,
            path,
            statusCode,
            elapsedMs,
            correlationId,
            userId
        );
    }
}
