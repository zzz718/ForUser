using Autofac;
using ForUser.Domains.Users;
using ForUser.SqlServer.Repository;

namespace ForUser.Modules
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var infraAssembly = typeof(UserRepository).Assembly;
            builder.RegisterAssemblyTypes(infraAssembly)
                   .Where(t => t.Name.EndsWith("Repository")
                            && !t.IsAbstract
                            && !t.IsInterface)
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }
    }
}
