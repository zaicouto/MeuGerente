using Modules.Orders.Domain.Enums;
using MongoDB.Bson;

namespace Modules.Orders.Application.DTOs;

public class OrderDto
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public required string TenantId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = OrderStatus.Pending.ToString();
    public List<OrderItemDto> Items { get; set; } = [];
}
