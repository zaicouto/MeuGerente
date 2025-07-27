using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Infrastructure.Filters;

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
