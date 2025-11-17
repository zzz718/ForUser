using Autofac;
using Castle.DynamicProxy;
using ForUser.HttpApi.Interceptors;

namespace ForUser.Modules
{
    public class InterceptorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 1. 注册拦截器 - 不依赖 IServiceProvider
            builder.RegisterType<UnitOfWorkInterceptor>()
                   .As<IInterceptor>()
                   .PropertiesAutowired()
                   .InstancePerLifetimeScope().AsSelf();

            Console.WriteLine(" InterceptorModule: UnitOfWorkInterceptor 注册成功");
        }
    }
}
