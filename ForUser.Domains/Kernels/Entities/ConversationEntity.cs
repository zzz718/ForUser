using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels.Entities
{
    public class ConversationEntity:Entity
    {
        public ConversationEntity()
        {
        }
        public ConversationEntity(long Id) : base(Id)
        {
        }

        public string Title { get; set; } = "New conversation";
        public ICollection<MessageEntity>Messages { get; set; } = new List<MessageEntity>();
    }


}
