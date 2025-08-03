namespace Modules.Orders.Application.DTOs;

public class OrderItemDto
{
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
}
