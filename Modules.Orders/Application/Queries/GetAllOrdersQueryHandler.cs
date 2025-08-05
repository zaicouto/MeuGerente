using AutoMapper;
using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Application.Extensions;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    : IRequestHandler<GetAllOrdersQuery, PaginatedOrdersResponseDto>
{
    public async Task<PaginatedOrdersResponseDto> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        request.PageNumber = Math.Clamp(request.PageNumber, 1, 1000);
        request.PageSize = Math.Clamp(request.PageSize, 1, 100);

        PaginatedList<Order> paginated = await orderRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        List<OrderResponseDto> dtoList = mapper.Map<List<OrderResponseDto>>(paginated.Items);

        return new PaginatedList<OrderResponseDto>(
            dtoList,
            paginated.TotalCount,
            paginated.PageNumber,
            paginated.PageSize
        ).ToOrdersResponseDto();
    }
}
