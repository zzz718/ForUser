using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Interceptors
{
    public interface IInterceptorFactory
    {
        IInterceptor CreateUnitOfWorkInterceptor();
    }
}
