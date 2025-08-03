using Modules.Orders.Domain.Entities;
using MongoDB.Driver;

namespace Modules.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    IMongoCollection<Order> Collection { get; }
    Task InsertAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}
