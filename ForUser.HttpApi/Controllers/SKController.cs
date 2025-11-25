using ForUser.Application.SK;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons;
using ForUser.Domains.Kernels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Controllers
{
    [ApiExplorerSettings(GroupName = ModuleCode.Permission)]
    public class SKController : AppControllerBase
    {
        private readonly ISemanticChatAppService _semanticChatAppService;
        private readonly ISKEmbeddingService _skembeddingService;
        public SKController(ISemanticChatAppService semanticChatAppService, ISKEmbeddingService skembeddingService)
        {
            _semanticChatAppService = semanticChatAppService;
            _skembeddingService = skembeddingService;
        }

        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<string> SendMessageAsync([FromBody] string userInput)
        {
            var result = await _semanticChatAppService.SendMessageAsync(userInput);
            return result;
        }
        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<string> SendMessageEmbeddingAsync(IFormFile file)
        {
            await _skembeddingService.MessageEmbeddingAsync(file);
            return "success";
        }
    }
}
