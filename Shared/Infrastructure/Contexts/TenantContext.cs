using Microsoft.AspNetCore.Http;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Contexts;

public class TenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public string? TenantId => accessor.HttpContext?.Items["TenantId"]?.ToString();

    /// <summary>
    /// Retorna informações sobre o contexto do locatário.
    /// </summary>
    /// <returns></returns>
    public string GetInfo()
    {
        HttpContext? httpContext = accessor.HttpContext;
        if (httpContext == null)
        {
            return "HttpContext is null";
        }
        if (httpContext.Items == null)
        {
            return "HttpContext.Items is null";
        }
        string tenantId = TenantId ?? "No TenantId set";
        return $"TenantContext: TenantId = {tenantId}, Request Path = {httpContext.Request.Path}";
    }
}
