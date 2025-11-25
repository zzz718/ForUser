using Autofac;
using Autofac.Extras.DynamicProxy;
using ForUser.Application.Users;
using ForUser.Domains.Attributes;
using ForUser.HttpApi.Interceptors;
using System.Reflection;

namespace ForUser.Modules
{

        public class ApplicationModule : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {
            var assembly = typeof(UserService).Assembly;

            var serviceTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Service") &&
                           !t.IsAbstract &&
                           !t.IsInterface)
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + serviceType.Name);

                if (interfaceType == null) continue;

                var hasUnitOfWorkMethods = interfaceType.GetMethods()
                    .Any(m => m.GetCustomAttribute<UnitOfWorkAttribute>() != null);

                if (hasUnitOfWorkMethods)
                {
                    Console.WriteLine($"🎯 Configuring interceptor for {interfaceType.Name}");

                    builder.RegisterType(serviceType)
                           .As(interfaceType)
                           .EnableInterfaceInterceptors()
                           .InterceptedBy(typeof(UnitOfWorkInterceptor))
                           .InstancePerLifetimeScope();
                }
                else
                {
                    Console.WriteLine($"📋 Registering {interfaceType.Name} without interceptor");

                    builder.RegisterType(serviceType)
                           .As(interfaceType)
                           .InstancePerLifetimeScope();
                }
            }
        }
        
    }
}
