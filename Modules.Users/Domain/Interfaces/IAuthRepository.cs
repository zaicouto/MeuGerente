using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Interfaces;

public interface IAuthRepository
{
    Task InsertAsync(User user);
    Task UpdateAsync(User user);
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task DeleteAsync(string id);
}
