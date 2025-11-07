using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.Cache
{
    public class UserCacheInfoEto
    {
        /// <summary>
        /// 当前用户所拥有的角色
        /// </summary>
        public List<UserRoleCacheEto> UserRoles { get; set; }

        /// <summary>
        /// 当前用户所拥有的方法权限
        /// </summary>
        public List<UserPermissionCacheEto> PermissionMethods { get; set; }



    }

    public class UserRoleCacheEto
    {

        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 组织Id
        /// </summary>
        public long OrgId { get; set; }

    }

    public class UserPermissionCacheEto
    {
        /// <summary>
        /// 方法ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 对象Id
        /// </summary>
        [JsonIgnore]
        public long ObjectId { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }

    }
}
