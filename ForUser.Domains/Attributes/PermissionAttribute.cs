using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Attributes
{
    /// <summary>
    /// 权限标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute: Attribute
    {
        /// <summary>
        /// 权限所属模块
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }


        public PermissionAttribute(string module, string code)
        {
            Module = module;
            Code = code;
        }
    }
}
