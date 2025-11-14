using ForUser.Domains.Commons.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer
{
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;
        private bool _disposed;
        private bool _transactionBegun;
        private readonly SemaphoreSlim _lock = new(1, 1);
        public EfCoreUnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool HasTransaction => _transaction != null && _transactionBegun;

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            await _lock.WaitAsync();
            try
            {
                if (_transaction == null)
                {
                    _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
                }
            }
            finally
            {
                _lock.Release();
            }
            
        }

        public async Task CommitTransactionAsync()
        {
            await _lock.WaitAsync();
            try
            {
                if (_transaction != null && _transactionBegun)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            finally
            {
                _lock.Release();
            }
            
        }

        public async Task RollbackTransactionAsync()
        {
            await _lock.WaitAsync();
            try
            {
                if (_transaction != null && _transactionBegun)
                {
                    await _transaction.RollbackAsync();
                    await _transaction.DisposeAsync();
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync();
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }
        // 同步释放
        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        // 异步释放
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await _lock.WaitAsync();
            try
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }

                if (_dbContext != null)
                {
                    await _dbContext.DisposeAsync();
                }

                _disposed = true;
            }
            finally
            {
                _lock.Release();
                _lock.Dispose();
            }
        }
    }
}
