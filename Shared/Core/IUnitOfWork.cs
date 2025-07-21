namespace Shared.Core;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
