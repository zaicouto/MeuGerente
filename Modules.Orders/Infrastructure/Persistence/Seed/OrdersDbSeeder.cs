using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Domain.Enums;

namespace Modules.Orders.Infrastructure.Persistence.Seed;

public static class OrdersDbSeeder
{
    public static async Task TruncateAsync(OrdersDbContext context)
    {
        // Delete all records from collection
        await context.Orders.DeleteManyAsync(FilterDefinition<Order>.Empty);
    }

    public static async Task SeedAsync(OrdersDbContext context)
    {
        bool hasAny = await context.Orders.Find(_ => true).AnyAsync();
        if (hasAny)
        {
            // Already populated. It skips and does nothing.
            return;
        }

        string tenantId = AdminCredentials.TenantId;

        List<Order> fakeOrders =
        [
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = tenantId,
                Status = OrderStatus.Pending,
                Items =
                [
                    new OrderItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductName = "Café Expresso",
                        Quantity = 2,
                        UnitPrice = 5.0m,
                    },
                    new OrderItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductName = "Pão de Queijo",
                        Quantity = 3,
                        UnitPrice = 3.5m,
                    },
                ],
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = tenantId,
                Status = OrderStatus.Delivered,
                Items =
                [
                    new OrderItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductName = "Sanduíche Natural",
                        Quantity = 1,
                        UnitPrice = 12.0m,
                    },
                ],
            },
        ];

        await context.Orders.InsertManyAsync(fakeOrders);
    }
}
