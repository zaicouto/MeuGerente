using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Domain.Enums;

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

        IPasswordHasher hasher = new BcryptPasswordHasher();

        User admin = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = SuperAdminCredentials.TenantId,
            Email = "admin@email.com",
            FirstName = "Admin",
            LastName = "User",
        };
        admin.SetPassword("Pass@12345", hasher);

        User user1 = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            Email = "user1@mail.com",
            FirstName = "User",
            LastName = "One",
        };
        user1.SetPassword("Pass@12345", hasher);

        User user2 = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            Email = "user2@mail.com",
            FirstName = "User",
            LastName = "Two",
        };
        user2.SetPassword("Pass@12345", hasher);

        List<User> fakeUsers = [admin, user1, user2];
        await context.Users.InsertManyAsync(fakeUsers);
    }
}
