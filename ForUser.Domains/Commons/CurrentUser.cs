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

        public bool IsAuthenticated { get; protected set; }

        public long Id { get; protected set; }

        public string Name { get; protected set; }

        public string? SurName { get; protected set; }

        public string? PhoneNumber { get; protected set; }

        public bool PhoneNumberVerified { get; protected set; }

        public string? Email { get; protected set; }

        public bool EmailVerified { get; protected set; }

        public long? TenantId { get; protected set; }

        public string[] Roles { get; protected set; }

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
