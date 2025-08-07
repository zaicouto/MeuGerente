using Microsoft.IdentityModel.Tokens;
using Shared.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Modules.Users.Infrastructure.Security;

/// <summary>
/// Utilitário para geração de tokens JWT.
/// </summary>
public class JwtHelper
{
    /// <summary>
    /// Gera um token JWT com base no email, tenantId e role do usuário.
    /// </summary>
    /// <param name="email">E-mail do usuário.</param>
    /// <param name="tenantId">ID do locatário.</param>
    /// <param name="role">Nível de acesso do usuário.</param>
    /// <returns>Token JWT gerado.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GenerateJwtToken(
        string email,
        string tenantId,
        UserRoles role = UserRoles.Default
    )
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role.ToString().ToLower()),
            new(CustomClaimTypes.TenantId, tenantId),
        ];
        string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT_KEY não está definido.");
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
