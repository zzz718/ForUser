using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    /// <summary>
    /// 审计
    /// </summary>
    public interface IAuditObject
    {
        /// <summary>
        ///  创建人Id
        /// </summary>
        public long CreateId { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改人编号
        /// </summary>
        public long? ModifierId { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string ModifilerName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifilcationTime { get; set; }


    }
}
