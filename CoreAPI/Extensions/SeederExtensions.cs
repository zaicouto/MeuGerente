using Modules.Orders.Infrastructure.Persistence;
using Modules.Orders.Infrastructure.Persistence.Seed;
using Modules.Users.Infrastructure.Persistence;
using Modules.Users.Infrastructure.Persistence.Seed;

namespace CoreAPI.Extensions;

public static class SeederExtensions
{
    public static async Task SeedTestDataAsync(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        OrdersDbContext ordersDb = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await OrdersDbSeeder.SeedAsync(ordersDb);

        AuthDbContext authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await AuthDbSeeder.SeedAsync(authDb);
    }
}
