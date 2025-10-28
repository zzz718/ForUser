using ForUser.Domains.Login.Dtos;
using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login
{
    public interface ILoginService
    {
        Task<UserEntity> Login(LoginInput input);
    }
}
