using ForUser.Domains;
using ForUser.Domains.Commons;
using ForUser.Domains.Login.Dtos;
using ForUser.Domains.Users;
using ForUser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userrepository;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginService(IUserRepository userrepository, IRefreshTokenService refreshTokenService)
        {
            _userrepository = userrepository;
            _refreshTokenService = refreshTokenService;
        }
        public async Task<UserEntity> Login(LoginInput input)
        {
            var user =await _userrepository.FindAsync(x=>x.Code==input.UserCode);
            if (user == null)
            {
                throw new Exception("用户未找到！");
            }
            if (MD5Helper.GetMD5String($"{input.Password}{user.PasswordHash}") == user.Password)
            {
                var loginToken = JwtServiceExtension.BuildToken(input);
                var refreshToken = MD5Helper.GetMD5String(loginToken.Token);
                await _refreshTokenService.SetRefreshTokenAsync(loginToken.UserCode, refreshToken,loginToken.claims);
                return user;
            }
            else
            {
                throw new Exception("用户名或密码错误！");
            }
        }
    }
}
