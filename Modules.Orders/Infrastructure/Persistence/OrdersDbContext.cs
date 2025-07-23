using Modules.Orders.Domain.Entities;
using MongoDB.Driver;

namespace Modules.Orders.Infrastructure.Persistence;

public class OrdersDbContext(IMongoClient client, string databaseName)
{
    private readonly IMongoDatabase _database = client.GetDatabase(databaseName);

    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    public IMongoCollection<OrderItem> OrderItems =>
        _database.GetCollection<OrderItem>("orderItems");
}
