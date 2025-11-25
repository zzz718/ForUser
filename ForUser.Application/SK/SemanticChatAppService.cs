using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ForUser.Domains.Kernels;
using Microsoft.Extensions.Logging;


namespace ForUser.Application.SK
{
    public class SemanticChatAppService : ISemanticChatAppService
    {
        private readonly KernelFactory _kernelFactory;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ChatHistory _history;
        private readonly PromptExecutionSettings _settings;
        private Kernel _kernel;
        public SemanticChatAppService(KernelFactory kernelFactory) // 注入 Singleton Kernel
        {
            _kernelFactory = kernelFactory;
            _kernel = _kernelFactory.GetKernelForModel("default");
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
