using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Queries;

public class GetOrderByIdQuery(string id) : IRequest<OrderDto>
{
    public string Id { get; set; } = id;
}
