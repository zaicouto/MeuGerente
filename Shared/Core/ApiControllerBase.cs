using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Base;

namespace Shared.Core;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected OkObjectResult Ok(object data, string message = "Requisição bem-sucedida.")
    {
        return base.Ok(new ApiResponse(true, message, data));
    }

    protected BadRequestObjectResult BadRequest(
        string message = "Requisição inválida.",
        object? errors = null
    )
    {
        return base.BadRequest(new ApiResponse(false, message, errors));
    }

    protected CreatedResult Created(
        string uri,
        object data,
        string message = "Recurso criado com sucesso."
    )
    {
        return base.Created(uri, new ApiResponse(true, message, data));
    }

    protected NotFoundObjectResult NotFound(string message = "Recurso não encontrado.")
    {
        return base.NotFound(new ApiResponse(false, message, null));
    }
}
