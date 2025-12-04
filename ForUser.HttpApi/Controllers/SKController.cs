using ForUser.Application.SK;
using ForUser.Application.SK.Dtos;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Permission("1", "创建对象")]
        //public async Task<string> SendMessageAsync([FromBody] string userInput)
        //{
        //    var result = await _semanticChatAppService.SendMessageAsync(userInput);
        //    return result;
        //}
        /// <summary>
        /// 发送文件向量化
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<string> SendMessageEmbeddingAsync(IFormFile file)
        {
            await _skembeddingService.MessageEmbeddingAsync(file);
            return "success";
        }
        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<string> StartConversationAsync([FromBody] CreateConversationRequest req)
        {
            var id = (await _semanticChatAppService.StartConversationAsync(req)).ToString();
            return id;
        }
        /// <summary>
        /// 获取会话列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Permission("1", "创建对象")]
        public async Task<IActionResult> GetListConversationsAsync(long userId)
        {
            var list = await _semanticChatAppService.GetListConversationsAsync(userId);
            return Ok(list);
        }
        /// <summary>
        /// 获取会话详情
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Permission("1", "创建对象")]
        public async Task<IActionResult> GetConversationAsync(long conversationId)
        {
            var conv = await _semanticChatAppService.GetConversationAsync(conversationId);
            if (conv == null) return NotFound();
            return Ok(conv);
        }

        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageRequest req)
        {
            var resp = await _semanticChatAppService.SendMessageAsync(req);
            return Ok(new { response = resp });
        }

        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<IActionResult> DeleteConversationAsync(long conversationId)
        {
            var ok = await _semanticChatAppService.DeleteConversationAsync(conversationId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost]
        [Permission("1", "创建对象")]
        public async Task<IActionResult> DeleteMessageAsync(long conversationId)
        {
            var ok = await _semanticChatAppService.DeleteMessageAsync(conversationId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
