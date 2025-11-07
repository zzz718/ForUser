using ForUser.Domains.Commons;
using ForUser.Domains.Commons.Cache;
using ForUser.Domains.Commons.Object;
using ForUser.Domains.Commons.ObjectFunc;
using ForUser.Domains.Commons.Role;
using ForUser.Domains.Commons.RoleObject;
using ForUser.Domains.Commons.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login
{
    public class MethodPermissionCheckService : IMethodPermissionCheckService
    {


        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICurrentUser _currentUser;

        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IObjectFuncRepository _objectFuncRepository;
        private readonly IRoleObjectRepository _roleObjectRepository;

        public MethodPermissionCheckService(IRefreshTokenService refreshTokenService, ICurrentUser currentUser, IUserRoleRepository userRoleRepository,  IObjectFuncRepository objectFuncRepository, IRoleObjectRepository roleObjectRepository)
        {
            _refreshTokenService = refreshTokenService;
            _currentUser = currentUser;
            _userRoleRepository = userRoleRepository;
            _objectFuncRepository = objectFuncRepository;
            _roleObjectRepository = roleObjectRepository;
        }

        public async Task<bool> CheckPermissionAync(string methodAttr)
        {
            List<string> info = new List<string>();
            // 先从缓存中取
            var cacheInfo =await  _refreshTokenService.GetUserCacheInfoAsync(methodAttr);
            
            if (cacheInfo != null)
            {
                info = cacheInfo;
            }
            else
            {
                // 缓存中没取到 从数据库中取
                info = await GetAndCacheUserInfoAsync();
                await _refreshTokenService.SetUserCacheInfoAsync(info);
            }

            if(info.Contains(methodAttr))return true;
            else return false;
        }


        private async Task<List<string>> GetAndCacheUserInfoAsync()
        {
            var userRoles = await _userRoleRepository.FindListAsync(x => x.UserId == _currentUser.Id);
            if(userRoles.Count == 0)
            {
                throw new Exception("用户没有角色");
            }
            // 取出用户的角色一直到对应的权限
            var roleIds = userRoles.Select(x => x.RoleId).ToList();
            var roleObjects = await _roleObjectRepository.FindListAsync(x => roleIds.Contains(x.RoleId));
            var objectIds = roleObjects.Select(x => x.ObjectId).ToList();
            var objectFuncs = await  _objectFuncRepository.FindListAsync(x => objectIds.Contains(x.ObjectId));

            var userPermissions = new List<string>();
            foreach(var objectFunc in objectFuncs)
            {
                userPermissions.Add(objectFunc.Method+objectFunc.Name);
            }
            //返回方法名+权限名
            return userPermissions;
        }
    }
}
