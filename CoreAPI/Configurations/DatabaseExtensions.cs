using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Repositories;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Repositories;
using MongoDB.Driver;
using Serilog;

namespace CoreAPI.Configurations;

public static class DatabaseExtensions
{
    public static void AddMongoDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("MONGO_CONNECTION_STRING não definida.");
        string? databaseName = Environment.GetEnvironmentVariable("MONGO_INITDB_DATABASE");
        if (string.IsNullOrEmpty(databaseName))
            throw new InvalidOperationException("MONGO_INITDB_DATABASE não definida.");

#if DEBUG
        Log.Information("Mongo Connection String: {ConnectionString}", connectionString);
#endif

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        // Database Contexts
        services.AddSingleton(sp => new OrdersDbContext(
            sp.GetRequiredService<IMongoClient>(),
            databaseName
        ));
        services.AddSingleton(sp => new UsersDbContext(
            sp.GetRequiredService<IMongoClient>(),
            databaseName
        ));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
