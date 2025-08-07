using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Exceptions;
using Modules.Orders.Domain.Interfaces;

namespace Modules.Orders.Application.Commands;

public class SoftDeleteOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<SoftDeleteOrderCommand>
{
    public async Task Handle(SoftDeleteOrderCommand request, CancellationToken cancellationToken)
    {
        Order order =
            await orderRepository.GetByIdAsync(request.OrderId)
            ?? throw new OrderNotFoundException();
        order.SoftDelete();
        await orderRepository.UpdateAsync(order);
    }
}
