using Modules.Orders.Domain.Entities;
using MongoDB.Driver;
using Shared.Domain.ValueObjects;

namespace Modules.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    IMongoCollection<Order> Collection { get; }
    Task InsertAsync(Order order);
    Task UpdateAsync(Order order);
    Task<PaginatedList<Order>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<Order?> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}
