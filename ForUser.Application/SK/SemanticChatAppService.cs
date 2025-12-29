using DocumentFormat.OpenXml.Wordprocessing;
using ForUser.Application.SK.Dtos;
using ForUser.Domains.Attributes;
using ForUser.Domains.Kernels;
using ForUser.Domains.Kernels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;


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
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly ConcurrentDictionary<string, ChatHistory> _userHistories = new();
        private readonly IMCPToolRepository _mcpToolRepository;
        public SemanticChatAppService(KernelFactory kernelFactory, IConversationRepository conversationRepository, IMessageRepository messageRepository, IKnowLedgeRepository knowLedgeRepository, ISKEmbeddingService embeddingService, IHttpClientFactory clientFactory, IConfiguration config, IMCPToolRepository mcpToolRepository)
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
            _clientFactory = clientFactory;
            _config = config;
            _mcpToolRepository = mcpToolRepository;
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
            //if (messages.Count < 1) throw new InvalidOperationException("no message");

            conv.Messages = messages;



            //判断是否应该使用知识库,以及使用哪个知识库
            var decision = await ShouldUseKnowledgeBaseAsync(req.Message);
            if (decision.UseKnowledgeBase)
            {
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
            }
            await _messageRepository.AddAsync(new MessageEntity
            {
                ConversationId = conv.Id,
                Role = "user",
                Content = req.Message,
                Timestamp = DateTime.Now,
                Sequence = conv.Messages.Count + 1
            });


            var assistantText = await GetLLMResponseAsync(conv);
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

        public async Task<string> SendMessageWithMCPAsync(SendMessageRequest req)
        {
            //用户输入先放入数据库
            var conv = await _conversationRepository.FindAsync(x => x.Id == req.ConversationId);
            if (conv == null) throw new ArgumentException("Conversation not found");
            if (conv.CreateId != req.UserId) throw new UnauthorizedAccessException();
            var messages = await _messageRepository.FindListAsync(x => x.ConversationId == req.ConversationId);
            //if (messages.Count < 1) throw new InvalidOperationException("no message");
            conv.Messages = messages;

            //向量化问题，然后通过向量距离获取较近的40条mcp工具做初筛
            var embedding = await _embeddingService.GetEmbeddingAsync(req.Message);

            var vector = new Vector(embedding.ToArray());

            var initial = await SearchMCPToolWithVectorAsync(vector);
            //通过关键词做筛选
            var keyWords = ExtractKeywords(req.Message);
            var candidates = new List<ToolCandidate>();
            //通过以上筛选计算得分
            foreach (var item in initial)
            {
                candidates.Add(new ToolCandidate
                {
                    ServiceName = item.ServiceName,
                    ServiceKey = item.ServiceKey,
                    ServiceInfo = item.ServiceInfo,
                    ServiceDescribe = item.ServiceDescribe,
                    Embedding = item.Embedding,
                    KeywordBoost = ComputeKeywordBoost(item, keyWords),
                    FinalScore = 0.7 * NormalizeSemanticScore(item.SemanticScore)+ 0.3*ComputeKeywordBoost(item, keyWords)
                });
            }
            //选出得分前5条MCPTool
            var filtered = candidates
                                    .OrderByDescending(c => c.FinalScore)
                                    .ThenByDescending(c => c.SemanticScore)
                                    .ThenByDescending(c =>c.KeywordBoost)
                                    .Take(5)
                                    .ToList(); 
            var TOOLS_JSON = JsonSerializer.Serialize(filtered.Select(c=>new 
            {
                c.ServiceName,
                c.ServiceDescribe,
                c.ServiceInfo,
                c.ServiceKey,
            }));
            var routerFunction = _kernel.CreateFunctionFromPrompt
                (promptTemplate: File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "Sk\\Skills\\Router\\Use_MCP\\prompt.txt")), functionName: "CheckUseMCPTool");
            //结合问题一起发给llm
            var modelResult = await routerFunction.InvokeAsync(_kernel, new()
            {
                ["USER_INPUT"] = req.Message,
                ["TOOLS_JSON"] = TOOLS_JSON
            });
            //将返回数据中的思考部分去掉，只保留返回数据中的MCP工具部分
            string separator = "\n</think>\n\n";

            
            int index = modelResult.ToString().IndexOf(separator);
            string json = index >= 0 ? modelResult.ToString().Substring(index + separator.Length) : string.Empty;
            //通过llm返回的MCP工具json，调用代理服务获取数据
            var client = _clientFactory.CreateClient();
            var getDataUrl = _config.GetValue<string>("Gateway:GetData");


            var response = await client.PostAsync(getDataUrl, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();


            await _messageRepository.AddAsync(new MessageEntity
            {
                ConversationId = conv.Id,
                Role = "user",
                Content = req.Message,
                Timestamp = DateTime.Now,
                Sequence = conv.Messages.Count + 1
            });
            await _messageRepository.AddAsync(new MessageEntity
            {
                ConversationId = conv.Id,
                Role = "system",
                Content = responseContent,
                Timestamp = DateTime.Now,
                Sequence = conv.Messages.Count + 1
            });
            var assistantText = await GetLLMResponseAsync(conv);
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
        /// 将用户输入，系统参数，mcp工具结果一起发送给LLM
        /// </summary>
        /// <param name="conv">用户输入，系统参数，mcp工具结果</param>
        /// <returns>LLM返回数据</returns>
        private async Task<string> GetLLMResponseAsync(ConversationEntity conv)
        {
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
            return chatMessage?.Content ?? "No response";
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
        private async Task<KnowledgeRouterDto> ShouldUseKnowledgeBaseAsync(string userMessage)
        {
            //判断使用哪个知识库
            var kbs =await SearchKnowledgeBaseAsync();
            string basePath = AppContext.BaseDirectory;
            var routerFunction = _kernel.CreateFunctionFromPrompt(promptTemplate: File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "Sk\\Skills\\Router\\Use_kb\\prompt.txt")),functionName: "CheckUseKB");
            var modelResult = await routerFunction.InvokeAsync(_kernel, new()
            {
                ["input"] = userMessage,
                ["kbs"] = kbs
            });
            //只获取think之外的结果
            string separator = "\n</think>\n\n";

            int index = modelResult.ToString().IndexOf(separator);
            string json = index >= 0 ? modelResult.ToString().Substring(index + separator.Length) : string.Empty;

            var decision = JsonSerializer.Deserialize<KnowledgeRouterDto>(json);

            return decision;
        }


        /// <summary>
        /// 通过http请求McpTooljson
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [UnitOfWork]
        public async Task GetMcpToolAsync(string serviceKey)
        {
            var client = _clientFactory.CreateClient();
            var mcpToolUrl = _config.GetValue<string>("Gateway:Url")+ serviceKey;
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync(mcpToolUrl);
                resp.EnsureSuccessStatusCode();
            }
            catch(Exception ex)
            {
                throw new Exception("获取MCP工具失败。  "+ex.Message);
            }
            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse($"[{json.Trim().TrimStart('[').TrimEnd(']')}]");
            // defensive: ensure it's an array-like string or already array.
            var ops = doc.RootElement;
            var entities = new List<MCPToolEntity>();
            foreach (var el in ops.EnumerateArray())
            {
                var entity = new MCPToolEntity();
                entity.ServiceName = ParseNameToRoute(el.GetProperty("name").GetString() ?? "").ToString();
                entity.ServiceKey = serviceKey;
                entity.ServiceInfo = BuildParametersSummary(el);
                entity.ServiceDescribe = el.GetProperty("description").GetString() ?? "";
                
                var vector = await _embeddingService.GetEmbeddingAsync(entity.ServiceDescribe);
                if (!string.IsNullOrWhiteSpace(entity.ServiceDescribe)) entity.Embedding = new Vector(vector.ToArray());
                entities.Add(entity);
            }
            await _mcpToolRepository.AddRangeAsync(entities);
        }
        [DisableUnitOfWork]
        private async Task<List<ToolCandidate>> SearchMCPToolWithVectorAsync(Vector vector,int topK=40)
        {
            var query = _mcpToolRepository.AsNoTracking();

            
            return [.. query.Select(s => new ToolCandidate
            {
                ServiceName = s.ServiceName,
                ServiceDescribe = s.ServiceDescribe,
                ServiceInfo = s.ServiceInfo,
                ServiceKey = s.ServiceKey,
                Embedding = s.Embedding,
                SemanticScore = 1 - s.Embedding.CosineDistance(vector)
            }) 
            .OrderByDescending(c=>c.SemanticScore)
            .Take(topK)];                       // 取前 topK 条
        }

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
        /// <summary>
        /// 计算关键词分数
        /// </summary>
        /// <param name="c"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private double ComputeKeywordBoost(ToolCandidate c, List<string> keywords)
        {
            if (keywords == null || keywords.Count == 0) return 0;
            var text = (c.ServiceName + " " + c.ServiceDescribe + " " + c.ServiceInfo).ToLowerInvariant();
            int hit = keywords.Count(k => text.Contains(k));
            double ratio = (double)hit / (double)keywords.Count;
            return Math.Min(1, ratio); // 0..1
        }
        private double NormalizeSemanticScore(double raw)
        {
            if (raw < 0) raw = 0;
            if (raw > 1) raw = 1;
            return raw;
        }
        // 简单关键词提取：去停用词、非中文/英文字符、按空格/标点切分
        private static readonly string[] StopWords = new[] { "the", "is", "请", "帮我", "获取", "查询" /* add more */ };
        /// <summary>
        /// 提取关键词
        /// </summary>
        /// <param name="text">用户输入问题</param>
        /// <returns></returns>
        private List<string> ExtractKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();
            // 小写
            var s = text.ToLowerInvariant();
            // 保留字母/数字/中文，替换其他为空格
            var matches = Regex.Matches(s,
        @"(?:(?:物料编码|货号|产品编号|物料号)\s*[为:=：]?\s*)?(\d{2}\.\d{2}\.\d{4}\.\d{2})",
        RegexOptions.IgnoreCase);

            return matches.Cast<Match>()
                         .Select(m => m.Groups[1].Value)
                         .Distinct()
                         .ToList();
        }
        /// <summary>
        /// 构建swagger接口的parameters
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        [DisableUnitOfWork]
        public string BuildParametersSummary(JsonElement el)
        {
            if (!el.TryGetProperty("parameters", out var p)) return "";
            if (p.ValueKind == JsonValueKind.Object)
            {
                if (p.TryGetProperty("$ref", out var rf) && rf.ValueKind == JsonValueKind.String)
                {
                    return $"ref:{rf.GetString()}";
                }
                if (p.TryGetProperty("type", out var t) && t.ValueKind == JsonValueKind.String && t.GetString() == "object")
                {
                    if (p.TryGetProperty("properties", out var props) && props.ValueKind == JsonValueKind.Object)
                    {
                        var items = new List<string>();
                        foreach (var prop in props.EnumerateObject())
                        {
                            var pname = prop.Name;
                            var ptype = "object";
                            if (prop.Value.TryGetProperty("type", out var pt) && pt.ValueKind == JsonValueKind.String) ptype = pt.GetString()!;
                            items.Add($"{pname}:{ptype}");
                            if (items.Count >= 8) break;
                        }
                        return string.Join(", ", items);
                    }
                }
            }
            return "";
        }
        public (string serviceKey, string route, string method) ParseNameToRoute(string name)
        {
            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return (parts.Length > 0 ? parts[0] : "", "/", parts.Length > 0 ? parts[^1] : "get");

            var serviceKey = parts[0];
            var method = parts[^1].ToLowerInvariant();

            string route;
            if (parts.Length >= 3 && parts[1].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                // join parts[1..^1]
                route = "/" + string.Join('/', parts, 1, parts.Length - 2);
            }
            else
            {
                route = "/" + string.Join('/', parts, 1, parts.Length - 2);
            }
            return (serviceKey, route, method);
        }


        public async Task<string> mmmm()
        {
            string json = "{\"should_call\":true,\"tool\":\"(permission, /api/material/get, get)\",\"arguments\":{\"id\":\"1175468\"},\"reason\":\"用户请求获取物料信息，需调用获取物料详情接口\",\"confidence\":0.95}";

            //通过llm返回的MCP工具json，调用代理服务获取数据
            var client = _clientFactory.CreateClient();
            var getDataUrl = _config.GetValue<string>("Gateway:GetData");
            var response = await client.PostAsync(getDataUrl, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var re = await response.Content.ReadAsStringAsync();
            return "";
        }

    }
}
