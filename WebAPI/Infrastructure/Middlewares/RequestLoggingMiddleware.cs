using System.Diagnostics;
using System.Security.Claims;
using Serilog;

namespace WebAPI.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var method = context.Request.Method;
            var path = context.Request.Path;
            var ip = context.Connection.RemoteIpAddress?.ToString();

            var correlationId =
                context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            Log.Information(
                "Request started: {Method} {Path} from {IP} | CorrelationId={CorrelationId} | UserId={UserId}",
                method,
                path,
                ip,
                correlationId,
                userId
            );

            await next(context);

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            Log.Information(
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
}
