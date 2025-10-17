using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public class SnowIdGenerator : IdWorker
    {
        public SnowIdGenerator() : base(6,6)
        {
        }
    }
}
