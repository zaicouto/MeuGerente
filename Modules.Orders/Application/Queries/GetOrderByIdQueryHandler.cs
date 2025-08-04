using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Exceptions;
using Modules.Orders.Domain.Interfaces;

namespace Modules.Orders.Application.Queries;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        Order? order = await orderRepository.GetByIdAsync(request.Id);
        return order == null
            ? throw new OrderNotFoundException()
            : new OrderDto
            {
                Id = order.Id,
                TenantId = order.TenantId,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Status = order.Status.ToString(),
                Items =
                [
                    .. order.Items.Select(i => new OrderItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    }),
                ],
            };
    }
}
