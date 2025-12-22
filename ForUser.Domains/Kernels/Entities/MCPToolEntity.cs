using ForUser.Domains.Commons;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class MCPToolEntity:IEntity
    {
        public virtual long Id { get; protected set; }
        public MCPToolEntity()
        {
            Id = SnowflakeId.NextId();
        }
        public MCPToolEntity(long Id)
        {
            this.Id = Id;
        }
        public string ServiceName { get; set; }
        /// <summary>
        /// 服务唯一标识
        /// </summary>
        public string ServiceKey { get; set; }
        /// <summary>
        /// 服务接口信息
        /// </summary>
        public string ServiceInfo { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string ServiceDescribe { get; set; }

        public Vector Embedding { get; set; }
    }
}
