using MongoDB.Bson.Serialization.Attributes;
using Shared.Core;

namespace Modules.Orders.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        [BsonElement("productName")]
        public required string ProductName { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
