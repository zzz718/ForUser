using ForUser.Domains;
using ForUser.Domains.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer
{
    public class ApplicationDbContext:DbContext
    {
        /// <summary>
        /// 数据库上下文构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="snowIdGenerator"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, SnowIdGenerator snowIdGenerator,ICurrentUser currentUser)
        : base(options)
        {
            _snowIdGenerator = snowIdGenerator;
            _currentUser = currentUser;
        }
        private readonly SnowIdGenerator _snowIdGenerator;
        private readonly ICurrentUser _currentUser;
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
        public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyConcepts();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        /// <summary>
        /// 判断是否需要添加审计字段数据
        /// </summary>
        private void ApplyConcepts()
        {
            foreach(var entry in ChangeTracker.Entries())
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
            if(entity is IEntity entityBase)
            {
                var currentId = (long?)entry.Property("Id").CurrentValue;
                if (currentId == null || currentId == 0)
                {
                    entry.Property("Id").CurrentValue = _snowIdGenerator.NextId();
                }
            }
            if (entity is IAuditObject audit)
            {
                audit.CreatorId = _currentUser.Id;
                audit.CreationTime = DateTime.Now;
                audit.CreatorName = _currentUser.Name;
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
                audit.ModificationTime = DateTime.Now;
                audit.ModifierName = _currentUser.Name;
            }
        }
    }
}
