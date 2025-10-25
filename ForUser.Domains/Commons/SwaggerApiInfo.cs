using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public class SwaggerApiInfo
    {
        /// <summary>
        /// URL前缀
        /// </summary>
        public string UrlPrefix { get; set; } = default!;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// <see cref="Microsoft.OpenApi.Models.OpenApiInfo"/>
        /// </summary>
        public OpenApiInfo OpenApiInfo { get; set; } = default!;
    }
}
