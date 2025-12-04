using ForUser.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.IdentityModel.Tokens;
using ForUser.Domains.Commons;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ForUser.PostgreSQL
{
    public class PostgreSQLEfCoreRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly PostgreSQLDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostgreSQLEfCoreRepositoryBase(PostgreSQLDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        // 在需要时通过属性获取当前用户
        public ICurrentUser _currentUser => _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ICurrentUser>();
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task<TEntity> AddWithReturnAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityArray = entities.ToArray();
            if (entityArray.IsNullOrEmpty())
            {
                return;
            }
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                return;
            //第三方库跳过了ef的验证，需要自己添加审计字段
            InsertBefore(entities.ToList());

            try
            {
                var bulkConfig = new BulkConfig { SetOutputIdentity = true};
                await _context.BulkInsertAsync(entities, bulkConfig);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
            {
                // 记录详细的错误信息
                Console.WriteLine("Bulk insert failed. Entity type: {EntityType}, Count: {Count}",
                    typeof(TEntity).Name, entities.Count());

                // 尝试获取表名映射
                var tableName = _context.Model.FindEntityType(typeof(TEntity))?.GetTableName();
                Console.WriteLine("Table name mapping: {TableName}", tableName ?? "Not found");

                throw new ApplicationException($"Bulk insert failed for {typeof(TEntity).Name}: {ex.Message}", ex);
            }
        }
        private void InsertBefore(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is IAuditObject audit)
                {
                    audit.CreateId = _currentUser.Id;
                    audit.CreateTime = DateTime.Now;
                    audit.CreateName = _currentUser.Name;
                }
            }
        }








        public Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
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

        public IQueryable<TEntity> AsNoTracking()
        {
            return _context.Set<TEntity>().AsNoTracking<TEntity>();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
