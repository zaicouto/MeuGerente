using MediatR;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using MongoDB.Bson;

namespace Modules.Users.Application.Commands;

public class CreateUserCommandHandler(IAuthRepository authRepository)
    : IRequestHandler<CreateUserCommand, string>
{
    public async Task<string> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        User user = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = "", // TODO: Set the tenant ID appropriately
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        await authRepository.InsertAsync(user);

        return user.Id;
    }
}
