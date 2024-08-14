using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using VehicleTaxonomy.Azure.Infrastructure;

namespace VehicleTaxonomy.Azure.Migrator;

public class HostServiceProvider
{
    public static ServiceProvider CreateServiceProvider()
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();

        services
            .AddInfrastructure(configuration)
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddScoped(s => TimeProvider.System);

        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<HostServiceProvider>()
            ;

        return configBuilder
            .AddEnvironmentVariables()
            .Build();
    }
}
