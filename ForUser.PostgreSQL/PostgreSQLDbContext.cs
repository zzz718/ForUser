using ForUser.Domains;
using ForUser.Domains.Commons;
using ForUser.Domains.Commons.UserRole;
using ForUser.Domains.Kernels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.PostgreSQL
{
    public class PostgreSQLDbContext: DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 数据库上下文构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="snowIdGenerator"></param>
        public PostgreSQLDbContext(DbContextOptions<PostgreSQLDbContext> options,  IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        // 在需要时通过属性获取当前用户
        public ICurrentUser _currentUser => _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ICurrentUser>();
        public DbSet<EmbeddingEntity> embeddingEntity{ get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Configure();
        }
        /// <summary>
        /// 在保存前添加Id和审计字段数据
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyConcepts();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        /// <summary>
        /// 在保存前添加Id和审计字段数据
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyConcepts();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        /// <summary>
        /// 判断是否需要添加审计字段数据
        /// </summary>
        private void ApplyConcepts()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        ApplyConceptsForAddedEntity(entry);
                        break;
                    case EntityState.Modified:
                        ApplyConceptsForModifiedEntity(entry);
                        break;
                }
            }
        }
        /// <summary>
        /// 添加 新增审计字段数据和Id
        /// </summary>
        /// <param name="entry"></param>
        private void ApplyConceptsForAddedEntity(EntityEntry entry)
        {
            var entity = entry.Entity;

            if (entity is IAuditObject audit)
            {
                audit.CreateId = _currentUser.Id;
                audit.CreateTime = DateTime.Now;
                audit.CreateName = _currentUser.Name;
            }
        }
        /// <summary>
        /// 添加 修改审计字段数据
        /// </summary>
        /// <param name="entry"></param>
        private void ApplyConceptsForModifiedEntity(EntityEntry entry)
        {
            var entity = entry.Entity;
            if (entity is IAuditObject audit)
            {
                audit.ModifierId = _currentUser.Id;
                audit.ModifilcationTime = DateTime.Now;
                audit.ModifilerName = _currentUser.Name;
            }
        }
    }
}
