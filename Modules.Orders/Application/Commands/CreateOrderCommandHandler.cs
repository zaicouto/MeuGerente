using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = Domain.Enums.OrderStatus.Pending,
            Items =
            [
                .. request.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }),
            ],
        };

        await orderRepository.AddAsync(order);
        await orderRepository.SaveChangesAsync();

        return order.Id;
    }
}
