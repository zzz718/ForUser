using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons
{
    /// <summary>
    /// abp中的当前登录用户信息
    /// </summary>
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        long Id { get; }

        string Name { get; }

        string? SurName { get; }

        string? PhoneNumber { get; }

        bool PhoneNumberVerified { get; }

        string? Email { get; }

        bool EmailVerified { get; }

        long? TenantId { get; }

        string[] Roles { get; }

        Claim? FindClaim(string claimType);

        Claim[] FindClaims(string claimType);

        Claim[] GetAllClaims();

        bool IsInRole(string roleName);
    }
}
