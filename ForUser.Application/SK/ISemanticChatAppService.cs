using ForUser.Application.SK.Dtos;
using ForUser.Domains.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK
{
    public interface ISemanticChatAppService
    {
        [DisableUnitOfWork]
        Task<long> StartConversationAsync(CreateConversationRequest req);
        //Task<string> SendMessageAsync(string userInput);
        Task<IEnumerable<ConversationDto>> GetListConversationsAsync(long userId);
        Task<ConversationDto?> GetConversationAsync(long conversationId);
        
        [UnitOfWork]
        Task<string> SendMessageAsync(SendMessageRequest req);
        [UnitOfWork]
        Task<bool> DeleteConversationAsync(long conversationId);
        [UnitOfWork]
        Task<bool> DeleteMessageAsync(long messageId);
        Task GetMcpToolAsync(string serviceKey);

        Task<string>SendMessageWithMCPAsync(SendMessageRequest req);
        Task<string> mmmm();

    }
}
