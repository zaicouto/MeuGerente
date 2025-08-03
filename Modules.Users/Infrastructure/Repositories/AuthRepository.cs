using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Modules.Users.Infrastructure.Repositories;

public class AuthRepository(AuthDbContext context) : IAuthRepository
{
    private readonly IMongoCollection<User> _users = context.Users;

    public async Task InsertAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
        user.UpdatedAt = DateTime.UtcNow;
        await _users.ReplaceOneAsync(filter, user);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, id);
        await _users.DeleteOneAsync(filter);
    }
}
