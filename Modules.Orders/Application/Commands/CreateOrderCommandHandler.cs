using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;
using Modules.Orders.Domain.Exceptions;
using Modules.Orders.Domain.Interfaces;
using MongoDB.Bson;
using Shared.Domain.Interfaces;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserContext userContext
) : IRequestHandler<CreateOrderCommand, string>
{
    public async Task<string> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        string tenantId =
            userContext.TenantId
            ?? throw new OrderBadRequestException("Não foi possível encontrar o ID do inquilino.");
        Order order = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            ClientId = request.ClientId,
        };
        order.UpdateTimestamps();
        order.UpdateStatus(OrderStatus.Pending);
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
        await orderRepository.InsertAsync(order);
        return order.Id;
    }
}
