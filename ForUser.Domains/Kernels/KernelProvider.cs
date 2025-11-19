using ForUser.Domains.Commons;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels
{
    public class KernelProvider
    {
        private readonly Kernel _kernel;
        /// <summary>
        /// 声明配置属性
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        public KernelProvider(IHttpClientFactory httpClientFactory)
        {
            var modelInfo = AppSettingsPlugIn.GetNode<ModelInfo>("ModelInfo");
            _httpClientFactory = httpClientFactory;
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelInfo.ModelId, modelInfo.Endpoint, modelInfo.Apikey, httpClient: _httpClientFactory.CreateClient("SemanticKernelLLM"));

            _kernel = builder.Build();
        }

        public Kernel GetKernel()
        {
            return _kernel.Clone();
        }
    }
}
