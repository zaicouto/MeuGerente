using Shared.Domain.Enums;

namespace Shared.Domain.Extensions;

public static class UserRolesExtensions
{
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
