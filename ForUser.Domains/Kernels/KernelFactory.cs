using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels
{
    public class KernelFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KernelFactory> _logger;
        private readonly ConcurrentDictionary<string, Kernel> _kernelCache = new();

        public KernelFactory(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<KernelFactory> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 获取指定模型的Kernel（带缓存）
        /// </summary>
        public Kernel GetKernelForModel(string modelName)
        {
            return _kernelCache.GetOrAdd(modelName, _ => CreateKernelForModel(modelName));
        }

        /// <summary>
        /// 创建新的Kernel实例（不缓存）
        /// </summary>
        public Kernel CreateNewKernelForModel(string modelName)
        {
            return CreateKernelForModel(modelName);
        }

        private Kernel CreateKernelForModel(string modelName)
        {
            // 从配置获取模型配置
            var modelSection = _configuration.GetSection($"ModelInfo:{modelName}");
            var modelConfig = modelSection.Get<ModelInfo>();

            if (modelConfig == null)
            {
                throw new ArgumentException($"Model configuration not found for: {modelName}");
            }

            _logger.LogInformation("Creating Kernel for model: {ModelName}, Endpoint: {Endpoint}",
                modelName, modelConfig.Endpoint);

            // 创建专用的HttpClient
            var httpClient = CreateHttpClientForModel(modelConfig);

            // 构建Kernel
            //var builder = Kernel.CreateBuilder()
            //    .AddOpenAIChatCompletion(
            //        modelId: modelConfig.ModelId,
            //        endpoint: new Uri(modelConfig.Endpoint),
            //        apiKey: modelConfig.ApiKey ?? string.Empty,
            //        httpClient: httpClient
            //    );
            var builder = Kernel.CreateBuilder();
            if (modelName.Equals("embedding", StringComparison.OrdinalIgnoreCase))
            {
                // ✅ 为embedding模型注册文本嵌入服务
                builder.AddOpenAITextEmbeddingGeneration(
                    modelId: modelConfig.ModelId,
                    apiKey: modelConfig.ApiKey ?? string.Empty,
                    httpClient: httpClient
                );
                _logger.LogInformation("Registered ITextEmbeddingGenerationService for model: {ModelName}", modelName);
            }
            else
            {
                // 为其他模型注册聊天完成服务
                builder.AddOpenAIChatCompletion(
                    modelId: modelConfig.ModelId,
                    endpoint: new Uri(modelConfig.Endpoint),
                    apiKey: modelConfig.ApiKey ?? string.Empty,
                    httpClient: httpClient
                );
                _logger.LogInformation("Registered ITextGenerationService for model: {ModelName}", modelName);
            }

            return builder.Build();
        }

        private HttpClient CreateHttpClientForModel(ModelInfo config)
        {
            // 从HttpClientFactory创建客户端
            var httpClient = _httpClientFactory.CreateClient("SemanticKernelLLM");
            return httpClient;
        }
    }
}
