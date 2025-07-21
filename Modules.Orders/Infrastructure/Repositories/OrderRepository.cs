using Microsoft.EntityFrameworkCore;
using Modules.Orders.Domain.Entities;
using Modules.Orders.Domain.Interfaces;
using Modules.Orders.Infrastructure.Persistence;

namespace Modules.Orders.Infrastructure.Repositories;

public class OrderRepository(OrdersDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid id) =>
        await dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

    public async Task AddAsync(Order order) => await dbContext.Orders.AddAsync(order);

    public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
}
