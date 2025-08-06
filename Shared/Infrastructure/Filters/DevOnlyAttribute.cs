using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Infrastructure.Filters;

/// <summary>
/// Filtro de ação que permite a execução apenas em ambientes de desenvolvimento.
/// </summary>
public class DevOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        IWebHostEnvironment? env =
            context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();

        if (env != null && !env.IsDevelopment() && env.EnvironmentName != "Testing")
        {
            context.Result = new NotFoundResult();
        }
    }
}
