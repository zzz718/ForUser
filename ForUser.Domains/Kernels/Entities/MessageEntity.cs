using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class MessageEntity:Entity
    {
        public MessageEntity() { }
        public MessageEntity(long id) : base(id) { }
        public long ConversationId { get; set; }
        public string Content { get; set; } = null!;
        public string Role { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Sequence { get; set; } // optional: helps ordering

    }
}
