using ForUser.Domains.Commons.Object;
using ForUser.Domains.Commons.ObjectFunc;
using ForUser.Domains.Commons.Role;
using ForUser.Domains.Commons.UserRole;
using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Cache
{
    public class CacheService:ICacheService
    {
        private readonly IObjectRepository _objectRepository;
        private readonly IObjectFuncRepository _objectFuncRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public CacheService(IObjectRepository objectRepository, IObjectFuncRepository objectFuncRepository, IRoleRepository roleRepository, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _objectRepository = objectRepository;
            _objectFuncRepository = objectFuncRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }



    }
}
