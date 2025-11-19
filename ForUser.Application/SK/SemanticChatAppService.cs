using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace ForUser.Application.SK
{
    public class SemanticChatAppService : ISemanticChatAppService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ChatHistory _history;
        private readonly PromptExecutionSettings _settings;
        public SemanticChatAppService(Kernel kernel) // 注入 Singleton Kernel
        {
            _kernel = kernel;
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>(); // 从 Kernel 获取服务

            _history = new ChatHistory();
            _settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions // 启用工具调用
            };
        }



        public async Task<string> SendMessageAsync(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Invalid input.";
            }

            _history.AddUserMessage(userInput);

            var chatMessage = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory: _history,
                executionSettings: _settings,
                kernel: _kernel // 传递 Kernel 以便工具调用时使用
            );

            var response = chatMessage.Content ?? "No response from AI.";
            _history.AddAssistantMessage(response);

            return response;
        }
    }
}
