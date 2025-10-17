using Autofac;
using Autofac.Extensions.DependencyInjection;
using ForUser.Domains.Commons;
using ForUser.Modules;
using Microsoft.Extensions.Configuration;
using Serilog;
using Snowflake.Core;
using System;

namespace ForUser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();
            // 2. 替换默认日志提供者为 Serilog
            builder.Host.UseSerilog();
            // 3. 注册 Autofac 作为服务提供者
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                // 注册 SnowIdGenerator 为单例
                containerBuilder.RegisterType<SnowIdGenerator>()
                                 .SingleInstance(); // ← Autofac 的 Singleton
                containerBuilder.RegisterModule<ApplicationModule>();
                containerBuilder.RegisterModule<InfrastructureModule>();
            });
            //添加httpAccessor用于获取当前请求上下文来现在只是用来获取登录用户信息
            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // 4. 启用 HTTP 请求日志（可选但推荐）
            app.UseSerilogRequestLogging(options =>
            {
                // 可选：自定义诊断上下文（例如添加用户ID）
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // 可选：添加其他字段（如 UserId）
                    var userId = httpContext.User.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        diagnosticContext.Set("UserId", userId);
                    }

                    // RequestId 已自动添加，无需手动设置！
                    // 实际上，Serilog 内部已执行：diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
                };
            });
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
