using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IFileGptService, OpenAiService>();
            services.AddScoped<OpenAiClientFactory>();

            return services;
        }
    }
}
