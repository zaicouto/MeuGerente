namespace Modules.Orders.Domain.Enums;

/// <summary>
/// Enumera os possíveis estados de um pedido.
/// </summary>
public enum OrderStatus
{
    Pending,
    Confirmed,
    Delivered,
    Cancelled,
}
