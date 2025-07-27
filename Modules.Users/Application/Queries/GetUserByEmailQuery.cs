using MediatR;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Application.Queries;

public class GetUserByEmailQuery(string email) : IRequest<User>
{
    public string Email { get; set; } = email;
}
