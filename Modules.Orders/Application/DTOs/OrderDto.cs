using Modules.Orders.Domain.Enums;
using MongoDB.Bson;

namespace Modules.Orders.Application.DTOs;

public class OrderDto
{
    public required string Id { get; set; }
    public required string TenantId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required string Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}

public class PublicOrderDto
{
    public required string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}
