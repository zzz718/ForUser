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
using System.Text.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SemanticKernel.Embeddings;
using Pgvector;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;


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
        private readonly IKnowLedgeRepository _knowLedgeRepository;
        private readonly ISKEmbeddingService _embeddingService;

        private readonly ConcurrentDictionary<string, ChatHistory> _userHistories = new();
        public SemanticChatAppService(KernelFactory kernelFactory, IConversationRepository conversationRepository, IMessageRepository messageRepository, IKnowLedgeRepository knowLedgeRepository, ISKEmbeddingService embeddingService) 
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
            _knowLedgeRepository = knowLedgeRepository;
            _embeddingService = embeddingService;
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
            if (conv == null) throw new ArgumentException("Conversation not found");
            if (conv.CreateId != req.UserId) throw new UnauthorizedAccessException();
            var messages = await _messageRepository.FindListAsync(x => x.ConversationId == req.ConversationId);
            if (messages.Count < 1) throw new InvalidOperationException("no message");

            conv.Messages = messages;



            //判断是否应该使用知识库,以及使用哪个知识库
            var decision = await ShouldUseKnowledgeBaseAsync(req.Message);
            if (decision.UseKnowledgeBase)
            {
                //向量化问题
                var  embedding = await _embeddingService.GetEmbeddingAsync(req.Message);

                var vector = new Vector(embedding.ToArray());
                //通过向量搜索
                var searchResult = await SearchContextAsync(vector, knowledgeBase: decision.KnowledgeBaseName);
                string searchResultStr = "";
                foreach (var item in searchResult)
                {
                    searchResultStr += item.Doc_Content.ToString() + "\n";
                }
                await _messageRepository.AddAsync(new MessageEntity
                {
                    ConversationId = conv.Id,
                    Role = "system",
                    Content = searchResultStr,
                    Timestamp = DateTime.Now,
                    Sequence = conv.Messages.Count + 1
                });
                await _messageRepository.AddAsync(new MessageEntity
                {
                    ConversationId = conv.Id,
                    Role = "user",
                    Content = req.Message,
                    Timestamp = DateTime.Now,
                    Sequence = conv.Messages.Count + 1
                });
            }
            else
            {
                await _messageRepository.AddAsync(new MessageEntity
                {
                    ConversationId = conv.Id,
                    Role = "user",
                    Content = req.Message,
                    Timestamp = DateTime.Now,
                    Sequence = conv.Messages.Count + 1
                });
            }
            //调用聊天服务
            var chatHistory = new ChatHistory();
            foreach (var m in conv.Messages.OrderBy(m => m.Sequence))
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
                
        
        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> DeleteConversationAsync(long conversationId)
        {
            var conversation = await _conversationRepository.FindAsync(x => x.Id == conversationId);
            if (conversation == null)throw new ArgumentException("Conversation not found");
            await _conversationRepository.DeleteAsync(conversation);
            return true;
        }
        /// <summary>
        /// 删除会话中的消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> DeleteMessageAsync(long messageId)
        {
            var message = await _messageRepository.FindAsync(x => x.Id == messageId);
            if (message == null) throw new ArgumentException("Message not found");
            await _messageRepository.DeleteAsync(message);
            return true;
        }

        /// <summary>
        /// 判断是否应该使用知识库
        /// </summary>
        /// <param name="userMessage"></param>
        /// <param name="conversationContext"></param>
        /// <returns></returns>
        private async Task<RouterDecision> ShouldUseKnowledgeBaseAsync(string userMessage)
        {
            //判断使用哪个知识库
            var kbs =await SearchKnowledgeBaseAsync();
            string basePath = AppContext.BaseDirectory;
            var routerFunction = _kernel.CreateFunctionFromPrompt(promptTemplate: File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Sk\\Skills\\Router\\Use_kb\\prompt.txt")),functionName: "CheckUseKB");
            var modelResult = await routerFunction.InvokeAsync(_kernel, new()
            {
                ["input"] = userMessage,
                ["kbs"] = kbs
            });
            string separator = "\n</think>\n\n";

            int index = modelResult.ToString().IndexOf(separator);
            string json = index >= 0 ? modelResult.ToString().Substring(index + separator.Length) : string.Empty;

            var decision = JsonSerializer.Deserialize<RouterDecision>(json);

            return decision;
        }



        /// <summary>
        /// 搜索知识库
        /// </summary>
        /// <returns></returns>
        [DisableUnitOfWork]
        private async Task<string> SearchKnowledgeBaseAsync()
        {
            var query = _knowLedgeRepository.AsNoTracking();
            var kbList = await query.Select(x => x.Doc_Name).Distinct().ToListAsync();
            var kbs = "";
            foreach (var kb in kbList)
            {
                kbs += kb + ",";
            }
            return kbs;
        }
        /// <summary>
        /// 搜索上下文
        /// </summary>
        /// <param name="queryEmbedding"></param>
        /// <param name="topK"></param>
        /// <param name="knowledgeBase"></param>
        /// <returns></returns>
        [DisableUnitOfWork]
        private async Task<List<EmbeddingEntity>> SearchContextAsync(Vector queryEmbedding, int topK = 5, string? knowledgeBase = null)
        {
            var query = _knowLedgeRepository.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(knowledgeBase))
            {
                query = query.Where(x => x.Doc_Name == knowledgeBase);
            }

            return await query
                .OrderBy(x => x.Embedding!.L2Distance(queryEmbedding))
                .Take(topK)
                .ToListAsync();
        }

    }
}
