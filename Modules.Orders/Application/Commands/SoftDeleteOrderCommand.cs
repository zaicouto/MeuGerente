using MediatR;

namespace Modules.Orders.Application.Commands;
public class SoftDeleteOrderCommand(string orderId) : IRequest
{
    public string OrderId { get; init; } = orderId;
}
