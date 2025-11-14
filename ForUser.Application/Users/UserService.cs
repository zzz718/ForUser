using AutoMapper;
using AutoMapper.Internal.Mappers;
using ForUser.Application.Common;
using ForUser.Application.Users.Dtos;
using ForUser.Domains.Commons;
using ForUser.Domains.Users;
using ForUser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ForUser.Application.Users
{
    public class UserService:IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly SnowIdGenerator _snowIdGenerator;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, SnowIdGenerator snowIdGenerator,IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _snowIdGenerator = snowIdGenerator;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<string> CreateUserAsync(CreateOrUpdateUserDto input)
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

            entity.PasswordHash = Guid.NewGuid().ToString("N");
            SetInitialPassword(entity);
            await _userRepository.AddAsync(entity);
            //if(await _userRepository.SaveAsync() > 0)
            //{

            //    return "保存成功";
            //}
            //else
            //{
            //    throw new Exception("保存失败");
            //}
            return "保存成功";
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PageUserDto>> GetListAsync(PageUserDto input)
        {
            throw new NotImplementedException();
        }

        public Task<ViewUserDto> GetUserForViewAsync(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 用户初始密码赋值
        /// </summary>
        /// <param name="entity"></param>
        private void SetInitialPassword(UserEntity entity)
        {

            var initialPassword = _configuration["InitialPassword"];
            if (string.IsNullOrWhiteSpace(initialPassword))
            {
                throw new Exception($"获取配置项：InitialPassword 失败，请联系管理员");
            }
            entity.Password = MD5Helper.GetMD5String($"{initialPassword}{entity.PasswordHash}");

        }
    }
}
