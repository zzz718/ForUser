using ForUser.Domains.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// 配置审计字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static EntityTypeBuilder<T> ConfigureAuditProperties<T>(this EntityTypeBuilder<T> builder)
            where T : class, IAuditObject
        {
            builder.Property(x => x.CreateId).HasComment("创建人Id").IsRequired();
            builder.Property(x => x.CreateName).HasComment("创建人名称").HasMaxLength(20).IsRequired();
            builder.Property(x => x.CreateTime).HasComment("创建时间").IsRequired();

            builder.Property(x => x.ModifierId).HasComment("最后修改人Id");
            builder.Property(x => x.ModifilerName).HasComment("最后修改人").HasMaxLength(20);
            builder.Property(x => x.ModifilcationTime).HasComment("最后修改时间");

            return builder;
        }
    }
}
