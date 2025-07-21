using Modules.Orders.Domain.Enums;
using Shared.Core;

namespace Modules.Orders.Domain.Entities;

public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
