using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using System.Security.Claims;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Salva o papel do usuário no contexto HTTP a partir do token JWT.
/// </summary>
public class RolesMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Claim? roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim is not null)
        {
            Console.WriteLine("Role do token: " + roleClaim);
            context.Items["Role"] = roleClaim.Value;
        }
        await next(context);
    }
}
