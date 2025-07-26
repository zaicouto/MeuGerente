using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Application.Queries;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Infrastructure.Filters;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/orders")]
[Consumes("application/json")]
[Produces("application/json")]
public class OrdersController(IMediator mediator, ILogger<OrdersController> logger) : ControllerBase
{
    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [SwaggerOperation(
        Summary = "Cria um novo pedido.",
        Description = "Cria um novo pedido com os dados informados."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Pedido criado com sucesso.")]
    public async Task<IActionResult> CreateAsync(CreateOrderCommand command)
    {
        var id = await mediator.Send(command);
        logger.LogInformation("Order created with ID: {OrderId}", id);
        return CreatedAtAction(nameof(GetByIdAsync), new { id }, id);
    }

    /// <summary>
    /// Busca um pedido pelo ID.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <returns>foo</returns>
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Busca um pedido pelo ID.",
        Description = "Exibe os detalhes de um pedido."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Pedido encontrado.")]
    public async Task<IActionResult> GetByIdAsync(string orderId)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(orderId));
        return order == null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// [Dev Only] Redefine a collection de pedidos.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [HttpPost("reset-db")]
    [DevOnly]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "[Dev Only] Redefine a collection de pedidos.",
        Description = "Apaga todos os documentos e depois popula a collection com dados de teste."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Reset da collection finalizado.")]
    public async Task<IActionResult> ResetDbAsync([FromServices] OrdersDbContext context)
    {
        await OrdersDbSeeder.TruncateAsync(context);
        await OrdersDbSeeder.SeedAsync(context);
        logger.LogInformation("Collection orders reseted.");
        return Ok("Reset da collection orders finalizado!");
    }
}
