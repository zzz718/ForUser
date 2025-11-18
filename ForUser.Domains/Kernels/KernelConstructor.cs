using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels
{
    public static class KernelConstructor
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
        {
            services.AddSingleton<KernelProvider>();

            services.AddScoped<Kernel>(sp =>
            {
                var provider = sp.GetRequiredService<KernelProvider>();

                return provider.GetKernel();
            });

            return services;
        }
    }
}
