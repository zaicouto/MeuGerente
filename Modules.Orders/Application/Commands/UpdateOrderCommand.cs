using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;

namespace Modules.Orders.Application.Commands;

public class UpdateOrderCommand : IRequest
{
    public string OrderId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];

}
