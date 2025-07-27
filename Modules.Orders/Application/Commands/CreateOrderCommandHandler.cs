using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using MongoDB.Bson;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, string>
{
    public async Task<string> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        Order order = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = "", // TODO: Set the tenant ID appropriately
            Status = Domain.Enums.OrderStatus.Pending,
            Items =
            [
                .. request.Items.Select(i => new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }),
            ],
        };

        await orderRepository.InsertAsync(order);

        return order.Id;
    }
}
