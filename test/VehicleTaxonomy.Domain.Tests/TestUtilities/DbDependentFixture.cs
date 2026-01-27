using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.CosmosDb;
using VehicleTaxonomy.Infrastructure;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Tests;

/// <summary>
/// Fixture for tests that need to access CosmosDb. The fixture
/// should be re-used across multiple tests, using unique keys to
/// scope data to each test.
/// </summary>
public sealed class DbDependentFixture : IAsyncLifetime
{
    private CosmosDbContainer? _container;

    /// <summary>
    /// We use a constant seed date for reproducible results.
    /// </summary>
    public static DateTimeOffset SeedDate => new(2024, 07, 16, 08, 23, 56, TimeSpan.Zero);

    public async ValueTask InitializeAsync()
    {
        var configuration = BuildConfiguration();
        var options = OptionsBinder.Bind<CosmosDbOptions>(configuration, CosmosDbOptions.SectionName);

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            // Note: we cannot pin a version because the CosmosDb image has an issue with trial expiry
            // that makes old container images fail.
            // See https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/264#issuecomment-3799371448
            _container = new CosmosDbBuilder("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest").Build();
            await _container.StartAsync();
        }

        ServiceProvider = CreateServiceProvider();
        using var scope = ServiceProvider.CreateScope();

        var cosmosDbContainerInitializer = scope.ServiceProvider.GetRequiredService<CosmosDbContainerInitializer>();
        await cosmosDbContainerInitializer.RebuildAsync(VehicleTaxonomyContainerDefinition.Instance);
    }

    /// <summary>
    /// A default service provider to use when no service customization is required.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Create a new custom service provider instance.
    /// </summary>
    /// <param name="additionalRegistration">
    /// Any additional service registration to apply after the default configuration.
    /// </param>
    public ServiceProvider CreateServiceProvider(Action<ServiceCollection>? additionalRegistration = null)
    {
        Action<IConfigurationBuilder>? additionalConfig = null;

        if (_container != null)
        {
            // If using test containers get the connection string from the container
            var cosmosDbConnectionString = _container.GetConnectionString();
            additionalConfig = b => b.AddInMemoryCollection([
                new($"{CosmosDbOptions.SectionName}:{nameof(CosmosDbOptions.ConnectionString)}", cosmosDbConnectionString),
                new($"{CosmosDbOptions.SectionName}:{nameof(CosmosDbOptions.UseLocalDb)}", "true")]);
        }
        var configuration = BuildConfiguration(additionalConfig);
        var services = new ServiceCollection();

        services
            .AddDomainTests(configuration)
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddScoped(s => new FakeTimeProvider(SeedDate))
            .AddScoped<TimeProvider>(s => s.GetRequiredService<FakeTimeProvider>());

        additionalRegistration?.Invoke(services);

        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration(Action<IConfigurationBuilder>? additionalConfig = null)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<DbDependentFixture>();
        additionalConfig?.Invoke(configBuilder);

        return configBuilder
            .AddEnvironmentVariables()
            .Build();
    }

    public async ValueTask DisposeAsync()
    {
        (ServiceProvider as IDisposable)?.Dispose();
        var task = _container?.DisposeAsync().AsTask();

        if (task is not null)
        {
            await task;
        }
    }
}
