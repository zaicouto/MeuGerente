using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Salva o papel do usuário no contexto HTTP a partir do token JWT.
/// </summary>
public class RolesMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            string token = authHeader["Bearer ".Length..];
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            Claim? roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Role);
            Console.WriteLine("Role do token : " + roleClaim);
            if (roleClaim != null)
            {
                context.Items["Role"] = roleClaim.Value;
            }
        }
        await next(context);
    }
}
