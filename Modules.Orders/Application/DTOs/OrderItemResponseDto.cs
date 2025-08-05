namespace Modules.Orders.Application.DTOs;

public record OrderItemResponseDto
{
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}
