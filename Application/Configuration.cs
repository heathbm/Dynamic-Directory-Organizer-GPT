using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<FilesService>();
            services.AddScoped<MappingsParser>();
            services.AddScoped<UserInputAndAiMappingsRefiningService>();

            services.AddHostedService<DynamicDirectoryOrganizerService>();

            return services;
        }
    }
}
