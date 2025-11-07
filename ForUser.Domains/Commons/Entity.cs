using ForUser.Domains.Commons;
using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains
{
    public class Entity : IEntity, IAuditObject
    {
        public virtual  long Id { get ;protected set; }
        public Entity(){}
        public  Entity(long Id)
        {
            this.Id = Id;
        }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public long CreateId { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改人Id
        /// </summary>
        public long? ModifierId { get; set; }
        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string? ModifilerName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifilcationTime { get; set; }
       
    }
}
