using Shared.Domain.Enums;

namespace Shared.Domain.Extensions;

public static class UserRolesExtensions
{
    /// <summary>
    /// Converte uma string para o enum UserRoles, se possível.
    /// </summary>
    /// <param name="role">Representação em string da role.</param>
    /// <returns>O valor convertido para o equivalente UserRoles.</returns>
    /// <exception cref="ArgumentException">Dispara se o parâmetro for uma role inválida.</exception>
    public static UserRoles ToUserRolesEnum(this string role)
    {
        return role.ToLowerInvariant() switch
        {
            "superadmin" => UserRoles.SuperAdmin,
            "admin" => UserRoles.Admin,
            "manager" => UserRoles.Manager,
            "user" => UserRoles.User,
            "default" => UserRoles.Default,
            "supervisor" => UserRoles.Supervisor,
            _ => throw new ArgumentException($"Role inválida: {role}"),
        };
    }
}
