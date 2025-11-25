

using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ForUser.Domains.Commons.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);

        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasTransaction { get; }
        // 可选：异步释放（.NET 6+）
        ValueTask DisposeAsync();
    }
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    { }
}
