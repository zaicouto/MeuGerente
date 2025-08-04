using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Exceptions;
using Modules.Orders.Domain.Interfaces;
using MongoDB.Bson;
using Shared.Domain.Interfaces;

namespace Modules.Orders.Application.Commands;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository, IRolesContext rolesContext)
    : IRequestHandler<UpdateOrderCommand>
{
    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        Order order =
            await orderRepository.GetByIdAsync(request.OrderId)
            ?? throw new OrderNotFoundException();

        order.UpdateStatus(request.Status, rolesContext.IsAdmin);
        order.UpdateItems(
            [
                .. request.Items.Select(i => new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }),
            ]
        );

        await orderRepository.UpdateAsync(order);
    }
}
