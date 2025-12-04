using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class SendMessageRequest
    {
        public long UserId { get; set; } 
        public long ConversationId { get; set; }
        public string Message { get; set; }
    }
}
