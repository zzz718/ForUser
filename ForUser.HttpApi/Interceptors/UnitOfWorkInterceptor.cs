using Castle.DynamicProxy;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons.UnitOfWork;
using ForUser.SqlServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Interceptors
{
    public class UnitOfWorkInterceptor : IAsyncInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnitOfWorkInterceptor> _logger;

        public UnitOfWorkInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // 延迟获取 logger，避免构造时依赖
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                            .CreateLogger<UnitOfWorkInterceptor>();
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            InterceptAsync(invocation).GetAwaiter().GetResult();
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            await InterceptAsync(invocation);
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            await InterceptAsync(invocation);
            return (TResult)invocation.ReturnValue!;
        }

        private async Task InterceptAsync(IInvocation invocation)
        {
            // 检查是否禁用工作单元
            var  method = invocation.MethodInvocationTarget ?? invocation.Method;
            var disableAttribute = method.GetCustomAttribute<DisableUnitOfWorkAttribute>() ??
                                               invocation.TargetType.GetCustomAttribute<DisableUnitOfWorkAttribute>();
            if (disableAttribute!= null)
            {
                invocation.Proceed();
                if (invocation.ReturnValue is Task task)
                {
                    await task;
                }
                return;
            }
            // 获取或创建工作单元
            var (unitOfWork, ownsUnitOfWork) = GetOrCreateUnitOfWork(invocation);
            try
            {
                // 检查事务特性
                var unitOfWorkResult = method.GetCustomAttribute<UnitOfWorkAttribute>()??
                                                          invocation.TargetType.GetCustomAttribute<UnitOfWorkAttribute>()??
                                                          new UnitOfWorkAttribute();
                // 开启事务
                if (unitOfWorkResult.IsTransactional&&!unitOfWork.HasTransaction)
                {
                    await unitOfWork.BeginTransactionAsync(unitOfWorkResult.IsolationLevel);
                }

                // 执行方法
                invocation.Proceed();

                if (invocation.ReturnValue is Task task)
                {
                    await task;
                }

                // 提交事务
                if (unitOfWorkResult.IsTransactional && unitOfWork.HasTransaction)
                {
                    await unitOfWork.CommitTransactionAsync();
                }
                // 保存更改（非事务性操作也需要保存）
                if (!unitOfWorkResult.IsTransactional)
                {
                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "UnitOfWorkInterceptor error");
                // 回滚事务
                if (unitOfWork.HasTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                }
                throw;
            }
            finally
            {
                // 只释放当前拦截器创建的工作单元
                if (ownsUnitOfWork)
                {
                    await unitOfWork.DisposeAsync();
                }
            }
        }


        private (IUnitOfWork,bool) GetOrCreateUnitOfWork(IInvocation invocation)
        {
            // 尝试从当前作用域获取工作单元
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext != null)
            {
                var unitOfWork = httpContextAccessor.HttpContext.RequestServices.GetService<IUnitOfWork>();
                if (unitOfWork != null)
                {
                    return (unitOfWork,false);
                }
            }

            // 创建新的工作单元
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            return (new EfCoreUnitOfWork(dbContext),true);
        }

    }
}
