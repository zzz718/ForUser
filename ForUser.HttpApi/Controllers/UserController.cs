using ForUser.Application.Common;
using ForUser.Application.Users;
using ForUser.Application.Users.Dtos;
using ForUser.Domains.Commons;
using ForUser.Domains.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace ForUser.HttpApi.Controllers
{
    [ApiExplorerSettings(GroupName = ModuleCode.Basic)]
    public class UserController: AppControllerBase
    {
        private readonly IUserService  _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Authorize]
        public async Task<MessageModel<string>> CreateUser([FromBody]CreateOrUpdateUserDto userDetail)
        {
            var result = await _userService.CreateUserAsync(userDetail);
            return MessageModel.OK("操作成功",result);
        }


       
    }
}
