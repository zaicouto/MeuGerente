using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Users.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Persistence.Seed;

namespace CoreAPI.Configurations;

public static class SeederExtensions
{
    public static async Task SeedTestDataAsync(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        // Pedidos
        OrdersDbContext ordersDb = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await OrdersDbSeeder.SeedAsync(ordersDb);

        // Usuários
        UsersDbContext usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await UsersDbSeeder.SeedAsync(usersDb);
    }
}
