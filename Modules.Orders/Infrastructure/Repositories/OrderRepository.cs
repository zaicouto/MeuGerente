using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Modules.Orders.Infrastructure.Repositories;

public class OrderRepository(OrdersDbContext context) : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders = context.Orders;

    public async Task InsertAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Id, order.Id);
        order.UpdatedAt = DateTime.UtcNow; // Update the date!
        await _orders.ReplaceOneAsync(filter, order);
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _orders.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.Id, id);
        await _orders.DeleteOneAsync(filter);
    }
}
