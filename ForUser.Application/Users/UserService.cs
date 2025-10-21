using AutoMapper;
using AutoMapper.Internal.Mappers;
using ForUser.Application.Common;
using ForUser.Application.Users.Dtos;
using ForUser.Domains.Commons;
using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Users
{
    public class UserService:IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly SnowIdGenerator _snowIdGenerator;

        public UserService(IUserRepository userRepository, SnowIdGenerator snowIdGenerator,IMapper mapper)
        {
            _userRepository = userRepository;
            _snowIdGenerator = snowIdGenerator;
            _mapper = mapper;
        }

        public async Task<MessageModel<CreateOrUpdateUserDto>> CreateUserAsync(CreateOrUpdateUserDto input)
        {
            var entity = _mapper.Map<CreateOrUpdateUserDto, UserEntity>(input);

            var entityExitByCode = await _userRepository.FindAsync(x => x.Code == input.Code);
            var existByUserMobileInfo = await _userRepository.FindAsync(x => x.Mobile == input.Mobile);

            if (entityExitByCode != null)
            {
                throw new Exception($"用户编码{entityExitByCode.Code}已存在");
            }
            if(existByUserMobileInfo!= null)
            {
                throw new Exception($"手机号{existByUserMobileInfo.Mobile}已存在");
            }

            
        }

        public Task<MessageModel<bool>> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MessageModel<List<PageUserDto>>> GetListAsync(PageUserDto input)
        {
            throw new NotImplementedException();
        }

        public Task<MessageModel<ViewUserDto>> GetUserForViewAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
