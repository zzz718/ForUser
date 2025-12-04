using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ForUser.Domains.Kernels;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using DocumentFormat.OpenXml.Spreadsheet;
using ForUser.Application.SK.Dtos;
using ForUser.Domains.Kernels.Entities;
using System.Formats.Asn1;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons;


namespace ForUser.Application.SK
{
    public class SemanticChatAppService : ISemanticChatAppService
    {
        private readonly KernelFactory _kernelFactory;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly PromptExecutionSettings _settings;
        private Kernel _kernel;
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;


        private readonly ConcurrentDictionary<string, ChatHistory> _userHistories = new();
        public SemanticChatAppService(KernelFactory kernelFactory, IConversationRepository conversationRepository, IMessageRepository messageRepository) 
        {
            _kernelFactory = kernelFactory;
            _kernel = _kernelFactory.GetKernelForModel("default");
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>(); // 从 Kernel 获取服务

            _settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions // 启用工具调用
            };
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
        }
        /// <summary>
        /// 创建会话,需要返回Id，所以不使用工作单元，单独save
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       [DisableUnitOfWork]
        public async  Task<long> StartConversationAsync(CreateConversationRequest request)
        {
            var conv = new ConversationEntity()
            {
                Title = request.Title ?? "New conversation"
            };
            if (!string.IsNullOrWhiteSpace(request.SystemPromat))
            {
                conv.Messages.Add(new MessageEntity()
                {
                    Role = "system",
                    ConversationId = conv.Id,
                    Content = request.SystemPromat ,
                    Timestamp = DateTime.Now,
                    Sequence = 1
                });
            }
            await _conversationRepository.AddAsync(conv);
            await _conversationRepository.SaveAsync();
            return conv.Id;
        }
        /// <summary>
        /// 通过用户Id获取会话列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ConversationDto>> GetListConversationsAsync(long userId)
        {
            var list = (await _conversationRepository.FindListAsync(x => x.CreateId == userId))
                .OrderByDescending(x=>x.ModifilcationTime);
            return list.Select(c=>new ConversationDto
            {
                Id = c.Id,
                Title = c.Title,
                Messages = c.Messages.OrderBy(m => m.Sequence).Select(m => new MessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    Sequence = m.Sequence
                })
            });
        }

        /// <summary>
        /// 通过会话Id获取会话详情
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<ConversationDto?>GetConversationAsync(long conversationId)
        {
            var conversation = await _conversationRepository.FindAsync(x => x.Id == conversationId);
            if(conversation == null)
            {
                return null;
            }
            return new ConversationDto
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Messages = conversation.Messages.OrderBy(m => m.Sequence).Select(m => new MessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    Sequence = m.Sequence
                })
            };
        }
        /// <summary>
        /// 先将用户输入存入数据库，然后调用SendMessageAsync方法进行聊天，然后将聊天结果存入数据库
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> SendMessageAsync(SendMessageRequest req)
        {
            //用户输入存入数据库
            var conv = await _conversationRepository.FindAsync(x => x.Id == req.ConversationId);
            if(conv ==null) throw new ArgumentException("Conversation not found");
            if(conv.CreateId != req.UserId) throw new UnauthorizedAccessException();
            var messages = await _messageRepository.FindListAsync(x => x.ConversationId == req.ConversationId);
            if(messages.Count <1) throw new InvalidOperationException("no message");

            conv.Messages = messages;


            var userMsg = new MessageEntity
            {
                ConversationId = conv.Id,
                Role = "user",
                Content = req.Message,
                Timestamp = DateTime.Now,
                Sequence = conv.Messages.Count + 1
            };
            await _messageRepository.AddAsync(userMsg);


            //调用聊天服务
            var chatHistory = new ChatHistory();
            foreach (var m in conv.Messages.OrderBy(m=>m.Sequence))
            {
                switch (m.Role)
                {
                    case "user":
                        chatHistory.AddUserMessage(m.Content);
                        break;
                    case "assistant":
                        chatHistory.AddAssistantMessage(m.Content);
                        break;
                    case "system":
                        chatHistory.AddSystemMessage(m.Content);
                        break;
                    default:
                        chatHistory.AddUserMessage(m.Content);
                        break;
                }
            }
            var chatMessage = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory: chatHistory,
                executionSettings: _settings,
                kernel: _kernel
            );
            //将聊天结果存入数据库
            var assistantText = chatMessage?.Content ?? "No response from AI.";
            var assistantMsg = new MessageEntity
            {
                ConversationId = conv.Id,
                Role = "assistant",
                Content = assistantText,
                Timestamp = DateTime.Now,
                Sequence = conv.Messages.Count + 1
            };
            await _messageRepository.AddAsync(assistantMsg);
            return assistantText;
        }

        public async Task<bool> DeleteConversationAsync(long conversationId)
        {
            var conversation = await _conversationRepository.FindAsync(x => x.Id == conversationId);
            if (conversation == null)throw new ArgumentException("Conversation not found");
            await _conversationRepository.DeleteAsync(conversation);
            return true;
        }

        public async Task<bool> DeleteMessageAsync(long messageId)
        {
            var message = await _messageRepository.FindAsync(x => x.Id == messageId);
            if (message == null) throw new ArgumentException("Message not found");
            await _messageRepository.DeleteAsync(message);
            return true;
        }



    }
}
