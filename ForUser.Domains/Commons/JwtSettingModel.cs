using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public class JwtSettingModel
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// 加密算法
        /// </summary>
        public string ENAlgorithm { get; set; }

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间    单位：秒
        /// </summary>
        public int ExpireSeconds { get; set; }
    }
}
