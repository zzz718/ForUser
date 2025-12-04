using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK.Dtos
{
    public class ConversationDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<MessageDto> Messages { get; set; } = Array.Empty<MessageDto>();
    }
}
