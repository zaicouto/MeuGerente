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

        string tenantId = SuperAdminCreds.TenantId;

        Order order1 = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            ClientId = ObjectId.GenerateNewId().ToString(),
        };
        order1.UpdateTimestamps();
        order1.UpdateStatus(OrderStatus.Pending);
        order1.UpdateItems(
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
            ]
        );

        Order order2 = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            ClientId = ObjectId.GenerateNewId().ToString(),
        };
        order2.UpdateTimestamps();
        order2.UpdateStatus(OrderStatus.Delivered);
        order2.UpdateItems(
            [
                new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = "Suco Natural",
                    Quantity = 1,
                    UnitPrice = 7.0m,
                },
                new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = "Bolo de Cenoura",
                    Quantity = 2,
                    UnitPrice = 4.0m,
                },
            ]
        );

        Order order3 = new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId,
            ClientId = ObjectId.GenerateNewId().ToString(),
            IsDeleted = true,
        };
        order3.UpdateTimestamps();
        order3.UpdateStatus(OrderStatus.Cancelled);
        order3.UpdateItems(
            [
                new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = "Água Mineral",
                    Quantity = 1,
                    UnitPrice = 2.0m,
                },
                new OrderItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductName = "Sanduíche Natural",
                    Quantity = 1,
                    UnitPrice = 8.0m,
                },
            ]
        );

        List<Order> fakeOrders = [order1, order2, order3];
        await context.Orders.InsertManyAsync(fakeOrders);
    }
}
