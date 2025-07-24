using Modules.Orders.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Orders.Domain.Entities;

public class Order : BaseEntity
{
    /// <summary>
    /// Status atual do pedido.
    /// </summary>
    [BsonElement("status")]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Lista de itens do pedido.
    /// </summary>
    [BsonElement("items")]
    public ICollection<OrderItem> Items { get; set; } = [];
}
