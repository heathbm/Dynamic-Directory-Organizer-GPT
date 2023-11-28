using Application.Interfaces;
using DynamicDirectoryOrganizerGpt.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Infrastructure;
using Models;
using Application;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureAppConfiguration((builder) => builder
    .AddJsonFile("appsettings.json", true, true)
    .AddEnvironmentVariables());

hostBuilder.ConfigureServices((context, services) =>
{
    services
        .AddOptions<Settings>()
        .Bind(context.Configuration.GetSection("settings"))
        .ValidateOnStart()
        .ValidateDataAnnotations();

    services.AddScoped<IConsoleService, ConsoleService>();
    services.ConfigureApplicationServices();
    services.ConfigureInfrastructureServices();
});

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

await hostBuilder.Build().RunAsync();