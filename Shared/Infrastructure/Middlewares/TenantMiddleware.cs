using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using System.Security.Claims;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Salva o ID do locatário no contexto HTTP a partir do token JWT.
/// </summary>
public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Claim? roleClaim = context.User.FindFirst(CustomClaimTypes.TenantId);
        if (roleClaim is not null)
        {
            Console.WriteLine("Tenant ID do token: " + roleClaim);
            context.Items["TenantId"] = roleClaim.Value;
        }
        await next(context);
    }
}
