using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Interceptors
{
    public class InterceptorFactory : IInterceptorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public InterceptorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IInterceptor CreateUnitOfWorkInterceptor()
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<UnitOfWorkInterceptor>>();
            return new UnitOfWorkInterceptor(_serviceProvider,logger);
        }
    }
}
