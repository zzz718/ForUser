using ForUser.Domains.Login.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public static  class JwtServiceExtension
    {
        public static void AddJwtService(this IServiceCollection services)
        {
            var jwtsetting = AppSettingsPlugIn.GetNode<JwtSettingModel>("JwtSetting");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtsetting.Issuer,
                    ValidAudience = jwtsetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting.SecurityKey)),
                };
            });
        }

        /// <summary>
        /// 反射获取字段
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<(string Name, object Value, string Type)> PropValuesType(this object obj)
        {
            List<(string a, object b, string c)> result = new List<(string a, object b, string c)>();

            var type = obj.GetType();
            var props = type.GetProperties();
            foreach (var item in props)
            {
                result.Add((item.Name, item.GetValue(obj), item.PropertyType.Name));
            }
            return result;
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="loginResult">登陆返回信息</param>
        /// <returns></returns>
        public static LoginOutPut BuildToken(LoginInput loginResult)
        {
            LoginOutPut result = new LoginOutPut();
            //获取配置
            var jwtsetting = AppSettingsPlugIn.GetNode<JwtSettingModel>("JwtSetting");

            //准备calims，记录登录信息
            var calims = loginResult.PropValuesType().Select(x => new Claim(x.Name, x.Value.ToString(), x.Type)).ToList();
            calims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            calims.Remove(calims.First(x => x.Type == "Password"));
            //创建header
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);

            //创建payload
            var payload = new JwtPayload(jwtsetting.Issuer, jwtsetting.Audience, calims, DateTime.Now, DateTime.Now.AddMinutes(jwtsetting.ExpireSeconds));

            //创建令牌 
            var token = new JwtSecurityToken(header, payload);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            result.ExpiresDate = token.ValidTo.AddHours(8).ToString();
            result.Token = tokenStr;
            result.UserName = loginResult.UserName;
            result.UserCode = loginResult.UserCode;
            result.claims = calims;
            return result;
        }
    }
}
