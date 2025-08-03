using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Contexts;

public class RolesContext(IHttpContextAccessor accessor) : IRolesContext
{
    public UserRoles Role
    {
        get
        {
            string? roleString = accessor.HttpContext?.Items["Role"]?.ToString();
            if (Enum.TryParse(roleString, true, out UserRoles parsedRole))
            {
                return parsedRole;
            }

            return UserRoles.Default;
        }
    }

    public bool IsAdmin => Role == UserRoles.Admin || Role == UserRoles.SuperAdmin;
}
