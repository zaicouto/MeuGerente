using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public List<OrderItemDto> Items { get; set; } = [];
}
