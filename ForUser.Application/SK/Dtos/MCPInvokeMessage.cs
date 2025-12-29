using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class MCPInvokeMessage
    {
        public string RequestId { get; set; } =Guid.NewGuid().ToString();

        /// <summary>
        /// (permission, /api/material/get, get)
        /// </summary>
        public string Tool { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }

        /// <summary>
        /// 调用来源（可选）
        /// </summary>
        public string? Caller { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
