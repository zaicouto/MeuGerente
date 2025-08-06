using MediatR;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Shared.Domain.Exceptions;

namespace Modules.Users.Application.Queries;

public class GetUserByEmailQueryHandler(IUsersRepository authRepository)
    : IRequestHandler<GetUserByEmailQuery, User>
{
    public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        User? user = await authRepository.GetByEmailAsync(request.Email);
        return user ?? throw new UnauthorizedException();
    }
}
