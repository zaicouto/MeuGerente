using MediatR;
using Modules.Orders.Application.DTOs;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQuery(int pageNumber, int pageSize) : IRequest<PaginatedList<OrderDto>>
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}
