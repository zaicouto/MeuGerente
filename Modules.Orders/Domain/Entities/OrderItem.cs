using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Orders.Domain.Entities;

public class OrderItem : BaseEntity
{
    /// <summary>
    /// Nome do produto associado ao item do pedido.
    /// </summary>
    [BsonElement("productName")]
    public required string ProductName { get; set; }

    /// <summary>
    /// Quantidade do produto no item do pedido.
    /// </summary>
    [BsonElement("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// Preço unitário do produto no item do pedido.
    /// </summary>
    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }
}
