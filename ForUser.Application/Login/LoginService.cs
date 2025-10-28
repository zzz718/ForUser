using ForUser.Domains;
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

        public LoginService(IUserRepository userrepository)
        {
            _userrepository = userrepository;
        }
        public async Task<UserEntity> Login(LoginInput input)
        {
            var user =await _userrepository.FindAsync(x=>x.Code==input.UserCode);
            if (user == null)
            {
                throw new Exception("用户未找到！");
            }
            if (MD5Helper.GetMD5String($"{input.Password}{user.PasswordHash}") == user.Password) return user;

            else
            {
                throw new Exception("用户名或密码错误！");
            }
        }
    }
}
