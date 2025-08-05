namespace Modules.Orders.Application.DTOs;

public record PaginatedOrdersResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public List<OrderResponseWithoutTenantIdDto> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
