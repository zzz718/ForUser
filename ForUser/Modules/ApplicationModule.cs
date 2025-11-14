using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using ForUser.Application.Users;
using ForUser.Domains.Attributes;
using ForUser.HttpApi.Controllers;
using ForUser.HttpApi.Interceptors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ForUser.Modules
{
    public class ApplicationModule:Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 1. 注册拦截器 - 使用正确的依赖解析
            builder.Register(c =>
            {
                // 从 Autofac 的上下文中获取 IServiceProvider
                var serviceProvider = c.Resolve<IServiceProvider>();
                var logger = serviceProvider.GetRequiredService<ILogger<UnitOfWorkInterceptor>>();

                return new UnitOfWorkInterceptor(serviceProvider);
            })
            .As<IAsyncInterceptor>()
            .InstancePerLifetimeScope();
            // 2. 批量注册服务并自动启用拦截
            RegisterServicesWithInterceptors(builder);
        }
        private void RegisterServicesWithInterceptors(ContainerBuilder builder)
        {
            var assembly = typeof(UserService).Assembly;

            // 获取所有服务类型（以Service结尾的类）
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service") &&
                           !t.IsAbstract &&
                           !t.IsInterface)
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                // 获取对应的接口（I + 服务名）
                var interfaceType = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + serviceType.Name);

                if (interfaceType == null) continue;

                // 检查该接口是否有方法标记了[UnitOfWork]特性
                var hasUnitOfWorkMethods = interfaceType.GetMethods()
                    .Any(m => m.GetCustomAttribute<UnitOfWorkAttribute>() != null);

                if (hasUnitOfWorkMethods)
                {
                    // 启用拦截器
                    builder.RegisterType(serviceType)
                           .As(interfaceType)
                           .EnableInterfaceInterceptors()  // ⭐ 启用接口拦截
                           .InterceptedBy(typeof(UnitOfWorkInterceptor))  // ⭐ 指定拦截器
                           .InstancePerLifetimeScope();
                }
                else
                {
                    // 普通服务，不需要拦截
                    builder.RegisterType(serviceType)
                           .As(interfaceType)
                           .InstancePerLifetimeScope();
                }
            }
        }
    }
}
