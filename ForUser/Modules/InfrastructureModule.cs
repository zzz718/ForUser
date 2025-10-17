using Autofac;
using ForUser.Domains.Users;

namespace ForUser.Modules
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var infraAssembly = typeof(IUserRepository).Assembly;
            builder.RegisterAssemblyTypes(infraAssembly)
                   .Where(t => t.Name.EndsWith("Repository")
                            && !t.IsAbstract
                            && !t.IsInterface)
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }
    }
}
