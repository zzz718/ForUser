using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK
{
    public interface ISemanticChatAppService
    {
        Task<string> SendMessageAsync(string userInput);
    }
}
