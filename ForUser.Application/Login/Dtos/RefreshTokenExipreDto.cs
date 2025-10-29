using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login.Dtos
{
    public class RefreshTokenExipreDto
    {
        /// <summary>
        /// 绝对过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 空闲过期时间
        /// </summary>
        public DateTime FreeExpireTime { get; set; }
    }
}
