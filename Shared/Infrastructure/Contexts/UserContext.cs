using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;
using Shared.Domain.Extensions;
using Shared.Domain.Interfaces;
using System.Security.Claims;

namespace Shared.Infrastructure.Contexts;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public UserRoles Role =>
        User?.FindFirst(ClaimTypes.Role)?.Value.ToUserRolesEnum()
        ?? UserRoles.Default;

    public string? TenantId => User?.FindFirst(CustomClaimTypes.TenantId)?.Value;

    public bool IsAdmin => Role == UserRoles.Admin || Role == UserRoles.SuperAdmin;

    public string GetInfo()
    {
        HttpContext? httpContext = accessor.HttpContext;
        if (httpContext == null)
        {
            return "HttpContext é nulo";
        }
        if (httpContext.User == null)
        {
            return "HttpContext.User é nulo";
        }
        string tenantId = TenantId ?? "TenantId não definido";
        return $"TenantContext: TenantId = {tenantId}, Caminho da requisição = {httpContext.Request.Path}";
    }
}
