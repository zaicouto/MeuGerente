using Microsoft.IdentityModel.Tokens;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Modules.Users.Infrastructure.Security;

public class JwtHelper
{
    public static string GenerateJwtToken(
        string email,
        string tenantId,
        UserRoles role = UserRoles.Default
    )
    {
        Claim[] claims =
        [
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(CustomClaimTypes.TenantId, tenantId),
            new(ClaimTypes.Role, role.ToString()),
        ];

        string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT_KEY environment variable is not set.");
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES"))
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
