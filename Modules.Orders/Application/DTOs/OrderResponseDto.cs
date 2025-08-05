namespace Modules.Orders.Application.DTOs;

public record OrderResponseDto
{
    public required string Id { get; set; }
    public required string TenantId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required string Status { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = [];
}

public record OrderResponseWithoutTenantIdDto
{
    public required string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Status { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = [];
}
