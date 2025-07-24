using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.Queries;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using WebAPI.Infrastructure.Filters;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(id));
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost("reset-db")]
    [DevOnly]
    public async Task<IActionResult> ResetDb([FromServices] OrdersDbContext context)
    {
        await OrdersDbSeeder.TruncateAsync(context);
        await OrdersDbSeeder.SeedAsync(context);
        return Ok("Reset da collection orders finalizado!");
    }
}
