using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Domain.Enums;

namespace Modules.Orders.Application.Commands;

public class UpdateOrderCommand : IRequest
{
    public string OrderId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public ICollection<OrderItemResponseDto> Items { get; set; } = [];
}
