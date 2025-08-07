using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.Infrastructure.Middlewares;

/// <summary>
/// Salva o ID do locatário no contexto HTTP a partir do token JWT.
/// </summary>
public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            string token = authHeader["Bearer ".Length..];
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            Claim? tenantClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == CustomClaimTypes.TenantId
            );
            Console.WriteLine("Tenant ID do token : " + tenantClaim);
            if (tenantClaim != null)
            {
                context.Items["TenantId"] = tenantClaim.Value;
            }
        }
        await next(context);
    }
}
