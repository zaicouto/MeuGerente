using Modules.Orders.Application.DTOs;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Application.Extensions;

public static class OrdersResponseDtoExtensions
{
    public static PaginatedOrdersResponseDto ToOrdersResponseDto(
        this PaginatedList<OrderResponseDto> paginated
    )
    {
        List<OrderResponseWithoutTenantIdDto> items =
        [
            .. paginated.Items.Select(order => new OrderResponseWithoutTenantIdDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Status = order.Status,
                Items = order.Items,
            }),
        ];
        return new PaginatedOrdersResponseDto
        {
            TenantId = paginated.Items.Count > 0 ? paginated.Items[0].TenantId : string.Empty,
            Items = items,
            PageNumber = paginated.PageNumber,
            PageSize = paginated.PageSize,
            TotalCount = paginated.TotalCount,
            TotalPages = paginated.TotalPages,
            HasPreviousPage = paginated.HasPreviousPage,
            HasNextPage = paginated.HasNextPage,
        };
    }
}
