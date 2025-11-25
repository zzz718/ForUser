using Autofac;
using ForUser.Domains.Kernels;
using ForUser.Domains.Users;
using ForUser.PostgreSQL.Repository;
using ForUser.SqlServer.Repository;

namespace ForUser.Modules
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var infraAssembly = typeof(UserRepository).Assembly;
            var pgSqlAssembly = typeof(KnowLedgeRepository).Assembly;
            builder.RegisterAssemblyTypes(infraAssembly)
                   .Where(t => t.Name.EndsWith("Repository")
                            && !t.IsAbstract
                            && !t.IsInterface)
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(pgSqlAssembly)
                   .Where(t => t.Name.EndsWith("Repository")
                            && !t.IsAbstract
                            && !t.IsInterface)
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.RegisterType<KernelFactory>()
       .As<KernelFactory>()
       .InstancePerLifetimeScope();
        }
    }
}
