using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehicleTaxonomy.Azure.Api;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureHostConfiguration(configurationBuilder =>
    {
        configurationBuilder
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddDomain(context.Configuration)
            .AddSingleton(TimeProvider.System)
            .AddSingleton(_ => OpenApiConfiguration.GetConfiguration());
    })
    .Build();

host.Run();
