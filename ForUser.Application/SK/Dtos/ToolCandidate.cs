using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class ToolCandidate
    {
        public string ServiceName { get; init; } = "";
        public string ServiceKey { get; init; } = "";
        public string ServiceInfo { get; init; } = "";
        public string ServiceDescribe { get; init; } = "";
        public Vector Embedding { get; set; }
        public double SemanticScore { get; init; }   // 0..1 from vector search (we'll normalize)
        public double KeywordBoost { get; set; }     // computed
        public double FinalScore { get; set; }       // composite
        public bool AuthRequired { get; init; }
        public bool Sensitive { get; init; }
        public long CallCount { get; init; }
        public DateTime? LastUsedAt { get; init; }
    }
}
