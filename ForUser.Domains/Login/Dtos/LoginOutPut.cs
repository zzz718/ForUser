using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Login.Dtos
{
    public class LoginOutPut
    {
        public long? Id { get; set; }
        public string UserCode { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Token过期时间
        /// </summary>
        public string? ExpiresDate { get; set; }

        public List<Claim> claims { get; set; }

    }
}
