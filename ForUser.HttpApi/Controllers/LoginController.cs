using ForUser.Application.Login;
using ForUser.Domains.Commons;
using ForUser.Domains.Login.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Controllers
{
    [ApiExplorerSettings(GroupName = ModuleCode.Permission)]
    public class LoginController:AppControllerBase
    {

        private readonly ILoginService _loginAppService;

        public LoginController(ILoginService loginAppService)
        {
            _loginAppService = loginAppService;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usercode"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetToken(long id,string usercode,string userName, string password)
        {
            var loginResult = JwtServiceExtension.BuildToken(new LoginInput {Id = id,UserCode = usercode ,UserName = userName, Password = password });
            return loginResult.Token ?? string.Empty;
        }

        /// <summary>
        /// 获取登陆人员信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public LoginOutPut GetLoginUserMsg()
        {
            StringValues s = new StringValues();
            var auth = Request.Headers.TryGetValue("Authorization", out s);
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception("登录信息失效");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(s.ToString().Replace($"{JwtBearerDefaults.AuthenticationScheme} ", ""));
            LoginOutPut loginResult = new()
            {
                Id = Convert.ToInt64(token.Claims.FirstOrDefault(f => f.Type == "Id").Value),
                UserName = token.Claims.FirstOrDefault(f => f.Type == "UserName").Value,
                Password = Convert.ToString(token.Claims.FirstOrDefault(f => f.Type == "Password").Value),
            };
            return loginResult;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<string> Login([FromBody]LoginInput loginInput)
        {
            var user = await _loginAppService.Login(loginInput);
            return GetToken(user.Id,user.Code, user.Name,user.Password);
        }
    }
}
