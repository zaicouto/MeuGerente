using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQuery(int pageNumber, int pageSize) : IRequest<PaginatedOrdersResponseDto>
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}
