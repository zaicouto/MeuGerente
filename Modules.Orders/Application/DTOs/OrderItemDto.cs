namespace Modules.Orders.Application.DTOs;

public class OrderItemDto
{
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
