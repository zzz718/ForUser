using ForUser.Application.Login.Dtos;
using ForUser.Domains.Commons;
using ForUser.Domains.Commons.Cache;
using ForUser.Domains.Commons.JsonConverters;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace ForUser.Application.Login
{
    public class RefreshTokenService : IRefreshTokenService
    {

        private readonly string userRefreshTokenKeyFormat = $"Key:{{0}}:User:{{1}}";
        private readonly string userRefreshTokenListKeyFormat = $"Key:{{0}}:User:{{1}}";


        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
        private readonly IDatabase _redis;
        private readonly TimeSpan _refreshTokenTtl = TimeSpan.FromDays(30);
        private readonly IConfiguration _configuration;
        private readonly ICurrentUser _currentUser;
        public RefreshTokenService(IConnectionMultiplexer redis, IConfiguration configuration, ICurrentUser currentUser)
        {
            _redis = redis.GetDatabase();
            _configuration = configuration;
            serializerOptions.Converters.Add(new ClaimConverter());
            _currentUser = currentUser;
        }

        public async Task SetRefreshTokenAsync(string userCode, string refreshToken, List<Claim> claims)
        {
            var refreshTokenExpireTimeStr = _configuration.GetSection("JwtSetting:RefreshTokenExpireTime").Value;

            var parseFlag = int.TryParse(refreshTokenExpireTimeStr, out var refreshTokenExpireTime);
            if (!parseFlag) refreshTokenExpireTime = 48; // 没读取到配置则默认48

            var refreshTokenKey = string.Format(userRefreshTokenKeyFormat, "APPName", refreshToken);
            var expireTimeSpan = TimeSpan.FromHours(refreshTokenExpireTime);

            await _redis.StringSetAsync(refreshTokenKey, JsonSerializer.Serialize(claims, serializerOptions), expireTimeSpan);

            var refreshTokenListKey = string.Format(userRefreshTokenListKeyFormat, "APPName", userCode);

            await _redis.HashSetAsync(refreshTokenListKey, refreshToken,value: JsonSerializer.Serialize(new RefreshTokenExipreDto
            {
                ExpireTime = DateTime.Now.Add(expireTimeSpan),
                FreeExpireTime = DateTime.Now.AddMinutes(60)
            }));
            await _redis.KeyExpireAsync(refreshTokenListKey, expireTimeSpan);

            var hashKeys =  _redis.HashScan(refreshTokenListKey);
            var removeKeys = hashKeys.Where(x => JsonSerializer.Deserialize<RefreshTokenExipreDto>(x.Value).ExpireTime < DateTime.Now).ToList();

            foreach (var key in removeKeys)
            {
                await _redis.HashDeleteAsync(refreshTokenListKey, key.Name);
            }
        }

        public async Task<List<string>> GetUserCacheInfoAsync(string userCode)
        {
            var key = string.Format(userRefreshTokenListKeyFormat, "UserPermission", _currentUser.Code);
            var val = await _redis.StringGetAsync(key);
            if (val.HasValue)
            {
                var userInfo = JsonSerializer.Deserialize<List<string>>(val);
                return userInfo;
            }
            return null;
        }

        public async Task SetUserCacheInfoAsync( List<string> userPermissions)
        {
            var key = string.Format(userRefreshTokenListKeyFormat, "UserPermission", _currentUser.Code);
            await _redis.StringSetAsync(key, JsonSerializer.Serialize(userPermissions));
        }
    }
}
