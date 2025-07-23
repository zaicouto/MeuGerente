using Modules.Orders.Domain.Entities;

namespace Modules.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    Task InsertAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}
