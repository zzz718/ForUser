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
        Task<MessageModel<CreateOrUpdateUserDto>> CreateUserAsync(CreateOrUpdateUserDto input);
        Task<MessageModel<bool>> DeleteUserAsync(int id);
        Task<MessageModel<ViewUserDto>> GetUserForViewAsync(int id);
        Task<MessageModel<List<PageUserDto>>> GetListAsync(PageUserDto input);
    }
}
