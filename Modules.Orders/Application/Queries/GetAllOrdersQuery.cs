using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQuery(int pageNumber, int pageSize) : IRequest<OrdersResponseDto>
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}
