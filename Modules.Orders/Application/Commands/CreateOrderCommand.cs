using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommand : IRequest<string>
{
    public List<OrderItemResponseDto> Items { get; set; } = [];
    public required string ClientId { get; set; }
}
