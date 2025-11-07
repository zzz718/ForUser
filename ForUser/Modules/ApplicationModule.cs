using Autofac;
using ForUser.Application.Users;
using ForUser.HttpApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ForUser.Modules
{
    public class ApplicationModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 注册应用服务（ITransientDependency / IScopedDependency 等）
            builder.RegisterAssemblyTypes(typeof(UserService).Assembly)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope(); // 默认 scoped
        }
    }
}
