using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using MongoDB.Driver;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Infrastructure.Repositories;

public class OrderRepository(OrdersDbContext context) : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders = context.Orders;
    public IMongoCollection<Order> Collection => _orders;

    public async Task InsertAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        FilterDefinition<Order> filter = NotDeletedFilter(
            Builders<Order>.Filter.Eq(x => x.Id, order.Id)
        );
        order.UpdateTimestamps();
        await _orders.ReplaceOneAsync(filter, order);
    }

    public async Task<PaginatedList<Order>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        FilterDefinition<Order> filter = NotDeletedFilter(Builders<Order>.Filter.Empty);
        IOrderedFindFluent<Order, Order> find = _orders
            .Find(filter)
            .SortByDescending(x => x.CreatedAt);

        return await PaginatedList<Order>.CreateAsync(
            find,
            pageNumber,
            pageSize,
            cancellationToken
        );
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        FilterDefinition<Order> filter = NotDeletedFilter(Builders<Order>.Filter.Eq(x => x.Id, id));
        return await _orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.Id, id);
        UpdateDefinition<Order> update = Builders<Order>
            .Update.Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _orders.UpdateOneAsync(filter, update);
    }

    private static FilterDefinition<Order> NotDeletedFilter(
        FilterDefinition<Order> additionalFilter
    )
    {
        return Builders<Order>.Filter.And(
            Builders<Order>.Filter.Eq(x => x.IsDeleted, false),
            additionalFilter
        );
    }
}
