using MediatR;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;
using Modules.Orders.Domain.Interfaces;
using MongoDB.Bson;
using Shared.Domain.Exceptions;
using Shared.Domain.Interfaces;

namespace Modules.Orders.Application.Commands;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ITenantContext tenantContext
) : IRequestHandler<CreateOrderCommand, string>
{
    public async Task<string> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        string tenantId =
            tenantContext.TenantId
            ?? throw new BadRequestException("Tenant ID não encontrado.");

        Order order = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            Status = OrderStatus.Pending,
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
