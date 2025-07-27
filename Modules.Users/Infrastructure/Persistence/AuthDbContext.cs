using Modules.Users.Domain.Entities;
using MongoDB.Driver;

namespace Modules.Users.Infrastructure.Persistence;

public class AuthDbContext(IMongoClient client, string databaseName)
{
    private readonly IMongoDatabase _database = client.GetDatabase(databaseName);

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
}
