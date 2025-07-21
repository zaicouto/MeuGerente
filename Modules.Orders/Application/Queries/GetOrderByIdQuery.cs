using MediatR;
using Modules.Orders.Application.DTOs;

namespace Modules.Orders.Application.Queries;

public class GetOrderByIdQuery(Guid id) : IRequest<OrderDto>
{
    public Guid Id { get; set; } = id;
}
