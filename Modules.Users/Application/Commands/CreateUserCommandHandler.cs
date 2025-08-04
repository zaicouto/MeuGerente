using MediatR;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using MongoDB.Bson;

namespace Modules.Users.Application.Commands;

public class CreateUserCommandHandler(IAuthRepository authRepository, IPasswordHasher hasher)
    : IRequestHandler<CreateUserCommand, string>
{
    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User user = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            TenantId = ObjectId.GenerateNewId().ToString(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        user.UpdateTimestamps();
        user.SetPassword(request.Password, hasher);

        await authRepository.InsertAsync(user);
        return user.Id;
    }
}
