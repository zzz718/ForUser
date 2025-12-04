using Autofac;
using Autofac.Core;
using Castle.DynamicProxy;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons.UnitOfWork;
using ForUser.PostgreSQL;
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

namespace ForUser.Interceptors
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        public readonly IServiceProvider _serviceProvider;
        public readonly ILogger<UnitOfWorkInterceptor> _logger;


        // 现在可以安全地接受依赖
        public UnitOfWorkInterceptor(IServiceProvider serviceProvider, ILogger<UnitOfWorkInterceptor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            _logger.LogInformation(" 拦截方法: {MethodName}", method.Name);

            // 检查是否禁用工作单元
            var disableAttribute = method.GetCustomAttributes<DisableUnitOfWorkAttribute>(true).Any() ||
               method.DeclaringType.GetCustomAttributes<DisableUnitOfWorkAttribute>(true).Any(); 



            if (disableAttribute )
            {
                invocation.Proceed();
                return;
            }

            // 同步执行核心逻辑（内部处理异步）
            InterceptCore(invocation).GetAwaiter().GetResult();
        }

        private async Task InterceptCore(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var unitOfWorkResult = method.GetCustomAttribute<UnitOfWorkAttribute>() ??
                                 invocation.TargetType.GetCustomAttribute<UnitOfWorkAttribute>() ??
                                 new UnitOfWorkAttribute();

            // 获取或创建工作单元
            var (unitOfWork, ownsUnitOfWork) = GetOrCreateUnitOfWork(invocation);

            try
            {
                // 开启事务（如果需要）
                if (unitOfWorkResult.IsTransactional && !unitOfWork.HasTransaction)
                {
                    
                    _logger.LogInformation("beginTransaction  为方法 {MethodName} 开启事务", method.Name);
                    await unitOfWork.BeginTransactionAsync(unitOfWorkResult.IsolationLevel);
                }

                // 执行业务方法
                invocation.Proceed();

                // 处理异步返回值
                if (invocation.ReturnValue is Task task)
                {
                    await task;
                }

                // 提交事务
                if (unitOfWorkResult.IsTransactional && unitOfWork.HasTransaction)
                {
                    var changes = await unitOfWork.SaveChangesAsync();
                    
                    _logger.LogInformation("commit  为方法 {MethodName} 提交事务,事务中保存了 {Changes} 个更改", method.Name, changes);
                    await unitOfWork.CommitTransactionAsync();
                }

                // 保存更改（非事务性操作）
                if (!unitOfWorkResult.IsTransactional)
                {
                    var changes = await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation(" 保存了 {Changes} 个更改", changes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " 工作单元执行失败: {MethodName}", method.Name);

                // 回滚事务
                if (unitOfWork.HasTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    _logger.LogInformation(" 事务已回滚");
                }
                throw;
            }
            finally
            {
                // 释放临时工作单元
                if (ownsUnitOfWork)
                {
                    await unitOfWork.DisposeAsync();
                    _logger.LogInformation("🧹 释放临时工作单元");
                }
            }
        }
        public void InterceptSynchronous(IInvocation invocation)
        {
            EnsureDependencies();
            InterceptAsync(invocation).GetAwaiter().GetResult();
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            EnsureDependencies();
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            EnsureDependencies();
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        private void EnsureDependencies()
        {
            if (_serviceProvider == null || _logger == null)
            {
                throw new InvalidOperationException("UnitOfWorkInterceptor 未正确初始化。请在 Autofac 配置中设置 ServiceProvider 和 Logger 属性。");
            }
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

            var dbContextType = GetDbContextType(invocation);
            // 尝试从当前作用域获取工作单元
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext != null)
            {

                if (dbContextType == 1)
                {
                    var unitOfWork = httpContextAccessor.HttpContext.RequestServices.GetService<IUnitOfWork<PostgreSQLDbContext>>();
                    if (unitOfWork != null)
                    {
                        return (unitOfWork, false);
                    }
                }
                else
                {
                    var unitOfWork = httpContextAccessor.HttpContext.RequestServices.GetService<IUnitOfWork<ApplicationDbContext>>();
                    if (unitOfWork != null)
                    {
                        return (unitOfWork, false);
                    }
                }

            }

            // 创建新的工作单元
            if (dbContextType == 1)
            {
                var dbContext = _serviceProvider.GetRequiredService<PostgreSQLDbContext>();
                return (new EfCoreUnitOfWork<PostgreSQLDbContext>(dbContext), true);
            }
            else
            {
                var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
                return (new EfCoreUnitOfWork<ApplicationDbContext>(dbContext), true);
            }
        }


        private int GetDbContextType(IInvocation invocation)
        {

            var target = invocation.InvocationTarget;
            var type = target.GetType();

            // BindingFlags 决定能够访问到 private 字段
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var fields = type.GetFields(flags);

            foreach (var field in fields)
            {
                var name = field.Name;          // 字段名
                var value = field.GetValue(target).ToString(); // 字段值

                if (value.Contains("PostgreSQL")) return 1;
            }
            return 0;


        }

    }
}
