using Autofac;
using Autofac.Extensions.DependencyInjection;
using ForUser.Application.Users.Profiles;
using ForUser.Domains.Commons;
using ForUser.HttpApi.Controllers;
using ForUser.Modules;
using ForUser.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ForUser.Application.Handle;
using Castle.DynamicProxy;
using ForUser.HttpApi.Interceptors;

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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                        ?? throw new InvalidOperationException("Connection string'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(factory =>
            {
                var redisConfig = builder.Configuration["Redis:Configuration"];
                // 或：var redisConfig = builder.Configuration.GetSection("Redis:Configuration").Value;

                if (string.IsNullOrEmpty(redisConfig))
                    throw new InvalidOperationException("Redis configuration is missing in appsettings.json under 'Redis:Configuration'.");

                var cfg = ConfigurationOptions.Parse(redisConfig);
                cfg.ResolveDns = true;
                return ConnectionMultiplexer.Connect(cfg);
            });
            //添加Automapper
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

            builder.Services.AddJwtService();
            //添加httpAccessor用于获取当前请求上下文来现在只是用来获取登录用户信息
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddRouting();
            builder.Services.AddControllers() .AddApplicationPart(typeof(UserController).Assembly);
            // ====== 添加 Swagger 服务 ======
            builder.Services.AddEndpointsApiExplorer(); // 必需：用于发现 API


            builder.Services.AddLoginAuthorization();
            builder.Services.AddSwaggerGen(options =>
            {
                // 遍历并应用Swagger分组信息
                SwaggerSetting.ApiInfos.ForEach(x =>
                {
                    options.SwaggerDoc(x.UrlPrefix, x.OpenApiInfo);
                });

                options.SwaggerDoc(SwaggerSetting.Version, new OpenApiInfo { Title = "Permission API", Version = SwaggerSetting.Version, });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiExplorerSettingsAttribute>()
                        .Select(attr => attr.GroupName);

                    if (docName.ToLower() == ModuleCode.Common && versions.FirstOrDefault() == null)
                    {
                        return true;
                    }
                    return versions.Any(v => v.ToString() == docName);
                });
                options.CustomSchemaIds(type => type.FullName);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                });

                //让swagger遵守jwt协议
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                     new OpenApiSecurityScheme
                     {
                        Reference = new OpenApiReference
                        {
                             Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                     },
                    new List<string>()
                    }
                 });
            });

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                
                containerBuilder.RegisterType<CurrentUser>().As<ICurrentUser>().InstancePerLifetimeScope();
                // 注册 SnowIdGenerator 为单例
                containerBuilder.RegisterType<SnowIdGenerator>()
                                 .SingleInstance(); // ← Autofac 的 Singleton
                containerBuilder.RegisterModule<InfrastructureModule>();

                containerBuilder.RegisterModule<InterceptorModule>();

                
                containerBuilder.RegisterModule<ApplicationModule>();

                
                containerBuilder.RegisterBuildCallback(container =>
                {
                    var interceptor = container.Resolve<IInterceptor>() as UnitOfWorkInterceptor;
                    if (interceptor != null)
                    {
                        var serviceProvider = container.Resolve<IServiceProvider>();
                        var logger = serviceProvider.GetRequiredService<ILogger<UnitOfWorkInterceptor>>();

                        Console.WriteLine(" 拦截器依赖注入完成");
                    }
                });

            });
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
            // 在 UseSwaggerUI 中：
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // 遍历分组信息，生成Json
                    SwaggerSetting.ApiInfos.ForEach(x =>
                    {
                        options.SwaggerEndpoint($"/swagger/{x.UrlPrefix}/swagger.json", x.Name);
                    });
                    options.RoutePrefix = "swagger";
                });
            }
            #region test
            app.Use(async (context, next) =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Authorization Header: {authHeader}");

                // 检查是否包含Bearer token
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader["Bearer ".Length..].Trim();
                    Console.WriteLine($"Token length: {token.Length}");
                    Console.WriteLine($"Token preview: {token[..20]}...");
                }

                await next();
            });
            #endregion
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            //app.MapRazorPages();
            
            // 重定向到 Swagger
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }
                await next();
            });

            app.Run();
        }
    }
}
