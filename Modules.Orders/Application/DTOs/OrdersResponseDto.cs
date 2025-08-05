namespace Modules.Orders.Application.DTOs;

public class OrdersResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public List<PublicOrderDto> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
