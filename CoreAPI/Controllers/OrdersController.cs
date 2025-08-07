using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Orders.Application.Commands;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Application.Queries;
using Modules.Orders.Domain.Enums;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Shared.Domain.Abstractions;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace CoreAPI.Controllers;

[Route("api/orders")]
public class OrdersController(IMediator mediator, Serilog.ILogger logger)
    : ApiControllerBase
{
    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [SwaggerOperation(
        Summary = "Cria um novo pedido.",
        Description = "Cria um novo pedido com os dados informados."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Pedido criado com sucesso.")]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateOrderCommand command)
    {
        string id = await mediator.Send(command);
        logger.Information("Pedido criado com ID: {OrderId}.", id);
        return Ok(new { NewOrderId = id });
    }

    /// <summary>
    /// Atualiza um pedido existente.
    /// </summary>
    [HttpPut("{orderId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Atualiza um pedido.",
        Description = "Atualiza os dados de um pedido existente."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Pedido atualizado com sucesso.")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] string orderId,
        [FromBody] UpdateOrderCommand command
    )
    {
        command.OrderId = orderId;
        await mediator.Send(command);
        logger.Information("Pedido atualizado com ID: {OrderId}.", orderId);
        return Ok(new { UpdatedOrder = true });
    }

    /// <summary>
    /// Marca um pedido como excluído logicamente (soft delete).
    /// </summary>
    [HttpDelete("{orderId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Marca um pedido como excluído.",
        Description = "Marca um pedido como excluído logicamente."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Pedido marcado como excluído.")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteAsync([FromRoute] string orderId)
    {
        await mediator.Send(new SoftDeleteOrderCommand(orderId));
        logger.Information("Pedido marcado como excluído com ID: {OrderId}.", orderId);
        return Ok(new { DeletedOrder = true });
    }

    /// <summary>
    /// Busca todos os pedidos com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<OrderResponseDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Lista pedidos com paginação.",
        Description = "Lista todos os pedidos de forma paginada."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de pedidos retornada.")]
    [Authorize]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null
    )
    {
        Console.WriteLine(status);
        PaginatedOrdersResponseDto orders = await mediator.Send(
            new GetAllOrdersQuery(pageNumber, pageSize)
        );
        logger.Information(
            "Encontrados {OrderCount} pedidos para a página {PageNumber} com {PageSize} itens por página.",
            orders.TotalCount,
            pageNumber,
            pageSize
        );
        return Ok(new { orders });
    }

    /// <summary>
    /// Busca um pedido pelo ID.
    /// </summary>
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Busca um pedido pelo ID.",
        Description = "Exibe os detalhes de um pedido."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Pedido encontrado.")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string orderId)
    {
        OrderResponseDto order = await mediator.Send(new GetOrderByIdQuery(orderId));
        logger.Information("Pedido encontrado com ID: {OrderId}.", orderId);
        return Ok(new { order });
    }

    /// <summary>
    /// [Dev Only] Redefine a coleção de pedidos.
    /// </summary>
    [HttpPost("resetdb")]
    [DevOnly]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "[Dev Only] Reinicia a coleção de pedidos.",
        Description = "Apaga todos os documentos e depois popula a coleção com dados de teste."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Reinício da coleção finalizado.")]
    public async Task<IActionResult> ResetDbAsync([FromServices] OrdersDbContext context)
    {
        await OrdersDbSeeder.TruncateAsync(context);
        await OrdersDbSeeder.SeedAsync(context);
        logger.Information("Coleção de pedidos reiniciada.");
        return Ok(new { Result = "Reinício da coleção de pedidos finalizada!" });
    }
}
