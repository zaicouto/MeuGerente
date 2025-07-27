using Modules.Users.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Modules.Users.Infrastructure.Persistence.Seed;

public static class AuthDbSeeder
{
    public static async Task TruncateAsync(AuthDbContext context)
    {
        // Delete all records from collection
        await context.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
    }

    public static async Task SeedAsync(AuthDbContext context)
    {
        bool hasAny = await context.Users.Find(_ => true).AnyAsync();
        if (hasAny)
        {
            // Already populated. It skips and does nothing.
            return;
        }

        string tenantId = ObjectId.GenerateNewId().ToString();

        List<User> fakeUsers =
        [
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = tenantId,
                Email = "user1@mail.com",
                Password = "Pass@12345",
                FirstName = "User",
                LastName = "One"
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = tenantId,
                Email = "user2@mail.com",
                Password = "Pass@12345",
                FirstName = "User",
                LastName = "Two"
            },
        ];

        await context.Users.InsertManyAsync(fakeUsers);
    }
}
