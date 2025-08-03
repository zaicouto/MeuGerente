using MongoDB.Bson;

namespace Modules.Orders.Application.DTOs;

public class OrderDto
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string TenantId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = default!;
    public List<OrderItemDto> Items { get; set; } = [];
}
