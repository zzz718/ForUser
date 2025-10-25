using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public static class SwaggerSetting
    {

        public static string Version = "v1.0";

        public static readonly List<SwaggerApiInfo> ApiInfos = new List<SwaggerApiInfo>();

        static SwaggerSetting()
        {
            ApiInfos.AddRange(SwaggerApiList.List);
        }

    }
}
