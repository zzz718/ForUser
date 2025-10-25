using ForUser.Application.Common;
using ForUser.Application.Users;
using ForUser.Application.Users.Dtos;
using ForUser.Domains.Commons;
using Microsoft.AspNetCore.Mvc;

namespace ForUser.HttpApi.Controllers
{
    [ApiExplorerSettings(GroupName = ModuleCode.Permission)]
    public class UserController: AppControllerBase
    {
        private readonly IUserService  _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<MessageModel<string>> CreateUser([FromBody]CreateOrUpdateUserDto userDetail)
        {
            var result = await _userService.CreateUserAsync(userDetail);
            return MessageModel.OK("操作成功",result);
        }
    }
}
