using ForUser.Application.Common;
using ForUser.Application.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Users
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(CreateOrUpdateUserDto input);
        Task<bool> DeleteUserAsync(int id);
        Task<ViewUserDto> GetUserForViewAsync(int id);
        Task<List<PageUserDto>> GetListAsync(PageUserDto input);
    }
}
