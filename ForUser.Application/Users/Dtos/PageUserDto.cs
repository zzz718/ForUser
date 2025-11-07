using ForUser.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Users.Dtos
{
    public class PageUserDto : Pagination
    {
        /// <summary>
        /// 账号，工号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别 0女 1男
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 状态 1启用 0禁用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 用户类型 0集团 1供应商
        /// </summary>
        public int Type { get; set; }
    }
}
