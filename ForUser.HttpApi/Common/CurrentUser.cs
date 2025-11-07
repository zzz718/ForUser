
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    public class CurrentUser : ICurrentUser
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext? CurrentHttpContext => _httpContextAccessor.HttpContext;

        private ClaimsPrincipal? CurrentUserClaims => CurrentHttpContext?.User;

        public bool IsAuthenticated => CurrentUserClaims?.Identity?.IsAuthenticated == true;

        public long Id
        {
            get
            {
                if (long.TryParse(CurrentUserClaims?.FindFirst("Id")?.Value, out var id))
                    return id;
                return 0;
            }
        }

        public string Code  => CurrentUserClaims?.FindFirst("userCode")?.Value ?? string.Empty;

        public string Name => CurrentUserClaims?.FindFirst("userName")?.Value ?? string.Empty;

        public string? SurName => CurrentUserClaims?.FindFirst("surname")?.Value;

        public string? PhoneNumber => CurrentUserClaims?.FindFirst("phone_number")?.Value;

        public bool PhoneNumberVerified =>
            bool.TryParse(CurrentUserClaims?.FindFirst("phone_number_verified")?.Value, out var verified) && verified;

        public string? Email => CurrentUserClaims?.FindFirst("email")?.Value;

        public bool EmailVerified =>
            bool.TryParse(CurrentUserClaims?.FindFirst("email_verified")?.Value, out var verified) && verified;

        public long? TenantId
        {
            get
            {
                if (long.TryParse(CurrentUserClaims?.FindFirst("tenant_id")?.Value, out var tid))
                    return tid;
                return null;
            }
        }

        public string[] Roles => CurrentUserClaims?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? Array.Empty<string>();

        
        public Claim? FindClaim(string claimType)
        {
            throw new NotImplementedException();
        }

        public Claim[] FindClaims(string claimType)
        {
            throw new NotImplementedException();
        }

        public Claim[] GetAllClaims()
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
