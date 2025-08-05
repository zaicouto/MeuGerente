using AutoMapper;
using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Application.Extensions;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    : IRequestHandler<GetAllOrdersQuery, OrdersResponseDto>
{
    public async Task<OrdersResponseDto> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        PaginatedList<Order> paginated = await orderRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        List<OrderDto> dtoList = mapper.Map<List<OrderDto>>(paginated.Items);

        return new PaginatedList<OrderDto>(
            dtoList,
            paginated.TotalCount,
            paginated.PageNumber,
            paginated.PageSize
        ).ToOrdersResponseDto();
    }
}
