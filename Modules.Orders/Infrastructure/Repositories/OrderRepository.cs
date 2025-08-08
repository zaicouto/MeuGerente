using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Enums;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;
using MongoDB.Driver;
using Shared.Domain.Abstractions;
using Shared.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Infrastructure.Repositories;

public class OrderRepository(OrdersDbContext context, IUserContext userContext) : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders = context.Orders;
    public IMongoCollection<Order> Collection => _orders;

    public async Task InsertAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        FilterDefinition<Order> filter = SameTenantAndNotDeletedFilter(
            Builders<Order>.Filter.Eq(x => x.Id, order.Id)
        );
        order.UpdateTimestamps();
        await _orders.ReplaceOneAsync(filter, order);
    }

    public async Task<PaginatedList<Order>> GetAllAsync(
        int pageNumber,
        int pageSize,
        OrderStatus? status,
        DateTime? createdAfter,
        CancellationToken cancellationToken
    )
    {
        List<FilterDefinition<Order>> filters = [];
        if (status.HasValue)
        {
            filters.Add(Builders<Order>.Filter.Eq(x => x.Status, status.Value));
        }
        if (createdAfter.HasValue)
        {
            filters.Add(Builders<Order>.Filter.Gte(x => x.CreatedAt, createdAfter.Value));
        }
        FilterDefinition<Order> filter = SameTenantAndNotDeletedFilter(
            Builders<Order>.Filter.And(filters)
        );
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
        FilterDefinition<Order> filter = SameTenantAndNotDeletedFilter(
            Builders<Order>.Filter.Eq(x => x.Id, id)
        );
        return await _orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<Order> filter = SameTenantAndNotDeletedFilter(Builders<Order>.Filter.Eq(x => x.Id, id));
        UpdateDefinition<Order> update = Builders<Order>
            .Update.Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);
        await _orders.UpdateOneAsync(filter, update);
    }

    private FilterDefinition<T> SameTenantAndNotDeletedFilter<T>(
        FilterDefinition<T> additionalFilter
    ) where T : EntityBaseWithTenant
    {
        return Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.IsDeleted, false),
            Builders<T>.Filter.Eq(x => x.TenantId, userContext.TenantId),
            additionalFilter
        );
    }
}
