using MongoDB.Bson;

namespace Modules.Orders.Application.DTOs;

public class OrderDto
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = default!;
    public List<OrderItemDto> Items { get; set; } = [];
}
