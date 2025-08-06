using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Repositories;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Repositories;
using MongoDB.Driver;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Contexts;

namespace CoreAPI.Extensions;

public static class DatabaseExtensions
{
    public static void AddMongoDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        string? databaseName = Environment.GetEnvironmentVariable("MONGO_INITDB_DATABASE");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("MONGO_CONNECTION_STRING não definida.");

        if (string.IsNullOrEmpty(databaseName))
            throw new InvalidOperationException("MONGO_INITDB_DATABASE não definida.");

#if DEBUG
        Console.WriteLine("Mongo Connection String: " + connectionString);
#endif

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        services.AddSingleton(sp => new OrdersDbContext(
            sp.GetRequiredService<IMongoClient>(),
            databaseName
        ));

        services.AddSingleton(sp => new AuthDbContext(
            sp.GetRequiredService<IMongoClient>(),
            databaseName
        ));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IRolesContext, RolesContext>();
    }
}
