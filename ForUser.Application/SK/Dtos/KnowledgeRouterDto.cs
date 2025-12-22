using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class KnowledgeRouterDto
    {
        public bool UseKnowledgeBase { get; set; }
        public string? KnowledgeBaseName { get; set; }
        public string Reason { get; set; }
    }
}
