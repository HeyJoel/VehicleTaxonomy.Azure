using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Infrastructure.DataImport;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithInjectionFactory<CosmosDbOptions>(configuration, CosmosDbOptions.SectionName);

        services.AddSingleton(c =>
        {
            var options = c.GetRequiredService<CosmosDbOptions>();
            var clientOptions = new CosmosClientOptions()
            {
                AllowBulkExecution = false,
                SerializerOptions = new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
            };

            if (options.UseLocalDb)
            {
                // TODO: Having issues running batches. Perhaps it will work when deployed?
                // also need to set up different connection for bulk as we don't want
                // to use it under normal operation
                clientOptions.ServerCertificateCustomValidationCallback = (c, h, e) => true;
                clientOptions.ConnectionMode = ConnectionMode.Gateway;
                clientOptions.LimitToEndpoint = true;
                clientOptions.RequestTimeout = TimeSpan.FromSeconds(30);
                clientOptions.MaxRetryAttemptsOnRateLimitedRequests = 0;
            }

            return new CosmosClient(options.ConnectionString, clientOptions);
        });

        services
            .AddTransient<IVehicleTaxonomyRepository, VehicleTaxonomyRepository>()
            .AddTransient<CosmosDbContainerInitializer>()
            .AddTransient<CsvDataImportJobRunner>();

        return services;
    }
}
