using ForUser.Domains.Commons.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login
{
    public interface IRefreshTokenService
    {
        Task SetRefreshTokenAsync(string userCode, string refreshToken, List<Claim> claims);
        Task<List<string>> GetUserCacheInfoAsync(string userCode);
        Task SetUserCacheInfoAsync(List<string> userPermissions);
    }
}
