using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.Role
{
    public class RoleEntity : Entity
    {
        public RoleEntity()
        {
        }
        public RoleEntity(long Id) : base(Id)
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public byte ProPerty { get; set; }

        public byte RoleType { get; set; }
        /// <summary>
        /// 创建组织
        /// </summary>
        public long CreateOrg { get; set; }
    }
}
