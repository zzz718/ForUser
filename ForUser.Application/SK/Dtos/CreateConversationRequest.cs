using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class CreateConversationRequest
    {
        public long UserId { get; set; }
        public string? Title { get; set; }
        public string? SystemPromat { get; set; }
    }
}
