using Modules.Orders.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Orders.Domain.Entities;

public class Order : BaseEntity
{
    [BsonElement("status")]
    public OrderStatus Status { get; set; }

    [BsonElement("items")]
    public ICollection<OrderItem> Items { get; set; } = [];
}
