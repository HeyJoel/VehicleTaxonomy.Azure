using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Testcontainers.CosmosDb;
using VehicleTaxonomy.Azure.Infrastructure;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Tests;

/// <summary>
/// Fixture for tests that need to access CosmosDb. The fixture
/// should be re-used across multiple tests, using unique keys to
/// scope data to each test.
/// </summary>
public sealed class DbDependentFixture : IAsyncLifetime
{
    private CosmosDbContainer? _container;

    /// <summary>
    /// We use a constant seed date for reproducable results.
    /// </summary>
    public DateTimeOffset SeedDate => new(2024, 07, 16, 08, 23, 56, TimeSpan.Zero);

    public async Task InitializeAsync()
    {
        var configuration = BuildConfiguration();
        var options = OptionsBinder.Bind<CosmosDbOptions>(configuration, CosmosDbOptions.SectionName);

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            _container = new CosmosDbBuilder().Build();
            await _container.StartAsync();
        }

        ServiceProvider = CreateServiceProvider();
        using var scope = ServiceProvider.CreateScope();

        var cosmosDbContainerInitializer = scope.ServiceProvider.GetRequiredService<CosmosDbContainerInitializer>();
        await cosmosDbContainerInitializer.RebuildAsync(VehicleTaxonomyContainerDefinition.Instance);
    }

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private ServiceProvider CreateServiceProvider()
    {
        Action<IConfigurationBuilder>? additionalConfig = null;

        if (_container != null)
        {
            // If using test containers get the connection string from the container
            var cosmosDbConnectionString = _container.GetConnectionString();
            additionalConfig = b => b.AddInMemoryCollection([
                    new($"{CosmosDbOptions.SectionName}:{nameof(CosmosDbOptions.ConnectionString)}", cosmosDbConnectionString),
                    new($"{CosmosDbOptions.SectionName}:{nameof(CosmosDbOptions.UseLocalDb)}", "true"),
                ]);
        }

        var configuration = BuildConfiguration(additionalConfig);
        var services = new ServiceCollection();

        services
            .AddDomainTests(configuration)
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddScoped(s => new FakeTimeProvider(SeedDate))
            .AddScoped<TimeProvider>(s => s.GetRequiredService<FakeTimeProvider>());

        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration(Action<IConfigurationBuilder>? additionalConfig = null)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<DbDependentFixture>();
        additionalConfig?.Invoke(configBuilder);

        return configBuilder
            .AddEnvironmentVariables()
            .Build();
    }

    public Task DisposeAsync()
    {
        var task = _container?.DisposeAsync().AsTask();

        return task ?? Task.CompletedTask;
    }
}
