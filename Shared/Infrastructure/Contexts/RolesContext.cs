using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using Shared.Domain.Extensions;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Contexts;

public class RolesContext(IHttpContextAccessor accessor) : IRolesContext
{
    public UserRoles Role
    {
        get
        {
            string? roleString = accessor.HttpContext?.Items["Role"]?.ToString();
            if (string.IsNullOrEmpty(roleString))
            {
                return UserRoles.Default;
            }

            return roleString.ToUserRolesEnum();
        }
    }

    public bool IsAdmin => Role == UserRoles.Admin || Role == UserRoles.SuperAdmin;
}
