using ForUser.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ForUser.SqlServer
{
    public class EfCoreRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity:class
    {
        protected readonly ApplicationDbContext _context;

        public EfCoreRepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public Task UpdateAsync(TEntity entity)
        {
             _context.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }

        //public async Task<IQueryable<TEntity>> GetAsNoTrackingQueryableAsync(string tag = null)
        //{

        //}
    }
}
