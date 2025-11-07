using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Handle
{
    public static class LoginAuthorizationExtesion
    {

        public static void AddLoginAuthorization(this IServiceCollection service)
        {
            service.AddAuthorization(option =>
            {
                option.AddPolicy("Permission",
                            policy => policy.Requirements.Add(new PermissionRequirement()));
            });
            service.AddScoped<IAuthorizationHandler, PermissionHandler>();
        }
    }
}
