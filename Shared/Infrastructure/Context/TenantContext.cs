using Microsoft.AspNetCore.Http;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Context
{
    public class TenantContext(IHttpContextAccessor accessor) : ITenantContext
    {
        public string? TenantId => accessor.HttpContext?.Items["TenantId"]?.ToString();
    }
}
