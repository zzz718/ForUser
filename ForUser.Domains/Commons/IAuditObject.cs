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
        long CreatorId { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        string CreatorName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// 修改人编号
        /// </summary>
        long? ModifierId { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        string ModifierName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime? ModificationTime { get; set; }


    }
}
