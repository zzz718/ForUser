using ForUser.Application.SK;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons;
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

        public SKController(ISemanticChatAppService semanticChatAppService)
        {
            _semanticChatAppService = semanticChatAppService;
        }

        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<string> SendMessageAsync ([FromBody] string userInput)
        {
            var result = await _semanticChatAppService.SendMessageAsync(userInput);
            return result;
        }
    }
}
