using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using MongoDB.Driver;
using Shared.ValueObjects;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetAllOrdersQuery, PaginatedList<OrderDto>>
{
    private readonly IMongoCollection<Order> _ordersCollection = orderRepository.Collection;

    public async Task<PaginatedList<OrderDto>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<Order>.Filter.Empty;
        var find = _ordersCollection.Find(filter).SortByDescending(x => x.CreatedAt);

        var paginated = await PaginatedList<Order>.CreateAsync(
            find,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        // Mapeia para DTO
        var dtoList = paginated
            .Items.Select(o => new OrderDto
            {
                Id = o.Id,
                TenantId = o.TenantId,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                Status = o.Status,
                Items =
                [
                    .. o.Items.Select(i => new OrderItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    }),
                ],
            })
            .ToList();

        return new PaginatedList<OrderDto>(
            dtoList,
            paginated.TotalCount,
            paginated.PageNumber,
            paginated.PageSize
        );
    }
}
