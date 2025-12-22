using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class MCPToolRouterDto
    {
        public bool ShouldCall { get; init; }
        public string Tool { get; init; } = "";
        public JsonElement Arguments { get; init; } = JsonDocument.Parse("{}").RootElement;
        public string Reason { get; init; } = "";
        public double Confidence { get; init; }
    }
}
