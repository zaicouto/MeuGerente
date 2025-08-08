using MediatR;
using Modules.Orders.Application.DTOs;
using Modules.Orders.Domain.Enums;

namespace Modules.Orders.Application.Queries;

public class GetAllOrdersQuery(
    int pageNumber,
    int pageSize,
    OrderStatus? status,
    DateTime? createdAfter
) : IRequest<PaginatedOrdersResponseDto>
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public OrderStatus? Status { get; set; } = status;
    public DateTime? CreatedAfter { get; set; } = createdAfter;
}
