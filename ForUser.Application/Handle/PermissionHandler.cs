using ForUser.Application.Common;
using ForUser.Application.Login;
using ForUser.Domains.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Handle
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IMethodPermissionCheckService _checkService;
        private readonly ILogger<PermissionHandler> _logger;
        public PermissionHandler(IMethodPermissionCheckService checkService, ILogger<PermissionHandler> logger)
        {
            _checkService = checkService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var filterContext = context.Resource as HttpContext;
            var result = await filterContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                try
                {
                    if (filterContext != null)
                    {
                        var endpoint = filterContext.GetEndpoint();
                        if (endpoint != null)
                        {
                            var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                            if(controllerActionDescriptor!= null)
                            {
                                var apiMethodInfo = controllerActionDescriptor.MethodInfo;
                                var permissionAttri = apiMethodInfo.GetCustomAttribute<PermissionAttribute>();
                                if (permissionAttri== null)
                                {
                                    context.Succeed(requirement);
                                    return;
                                }
                                var permissionModule = permissionAttri.Module;
                                var permissionCode = permissionAttri.Code;

                                var check = await _checkService.CheckPermissionAync($"{permissionAttri.Module}{permissionCode}");
                                if (!check)
                                {
                                    filterContext.Response.StatusCode = (int)HttpStatusCode.OK;
                                    await filterContext.Response.WriteAsJsonAsync(MessageModel.Fail("接口权限校验不通过，请联系管理员", (int)HttpStatusCode.Forbidden));
                                    return;
                                }
                                context.Succeed(requirement);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"接口权限校验时出现异常:{ex.Message}");
                    filterContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    await filterContext.Response.WriteAsJsonAsync(MessageModel.Fail("接口权限校验时出现异常", (int)HttpStatusCode.Forbidden));
                }
            }

        }
    }
}
