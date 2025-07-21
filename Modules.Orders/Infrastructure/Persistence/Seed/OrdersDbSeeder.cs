using Microsoft.EntityFrameworkCore;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;

namespace Modules.Orders.Infrastructure.Persistence.Seed;

public static class OrdersDbSeeder
{
    public static async Task TruncateAsync(OrdersDbContext context)
    {
        if (!await context.Orders.AnyAsync())
        {
            // There are no data, nothing to truncate.
            return;
        }

        // Truncate the Orders table.
        await context.Database.ExecuteSqlRawAsync("DELETE from Orders");
    }

    public static async Task SeedAsync(OrdersDbContext context)
    {
        if (await context.Orders.AnyAsync())
        {
            // There are already data, don't seed again.
            return;
        }

        var fakeOrders = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductName = "Café Expresso",
                        Quantity = 2,
                        UnitPrice = 5.0m,
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductName = "Pão de Queijo",
                        Quantity = 3,
                        UnitPrice = 3.5m,
                    },
                ],
            },
            new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Delivered,
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductName = "Sanduíche Natural",
                        Quantity = 1,
                        UnitPrice = 12.0m,
                    },
                ],
            },
        };

        await context.Orders.AddRangeAsync(fakeOrders);
        await context.SaveChangesAsync();
    }
}
