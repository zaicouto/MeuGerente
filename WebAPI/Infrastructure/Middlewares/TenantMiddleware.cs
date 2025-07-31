using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Infrastructure.Middlewares
{
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

                Claim? tenantClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenantId");
                if (tenantClaim != null)
                {
                    context.Items["TenantId"] = tenantClaim.Value;
                }
            }

            await next(context);
        }
    }
}
