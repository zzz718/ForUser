using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public static class SwaggerApiList
    {
        public static List<SwaggerApiInfo> List = new List<SwaggerApiInfo>()
        {
            new SwaggerApiInfo
            {
                UrlPrefix = ModuleCode.Common,
                Name = "系统公共",
                OpenApiInfo = new OpenApiInfo
                {
                    Title = "系统公共",
                    Version="1.0",
                    Description = ""
                }
            },
            new SwaggerApiInfo
            {
                UrlPrefix = ModuleCode.Permission,
                Name = "权限管理",
                OpenApiInfo = new OpenApiInfo
                {
                    Title = "权限管理",
                    Version="1.0",
                    Description = ""
                }
            },
            new SwaggerApiInfo
            {
                UrlPrefix= ModuleCode.Basic,
                Name = "基础资料",
                OpenApiInfo = new OpenApiInfo
                {
                    Title = "基础资料",
                    Version = "1.0",
                    Description=""
                }
            }
        };
    }
}
