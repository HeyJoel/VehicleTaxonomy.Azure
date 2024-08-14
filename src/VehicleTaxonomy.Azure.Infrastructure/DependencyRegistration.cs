using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Infrastructure.DataImport;
using VehicleTaxonomy.Azure.Infrastructure.Db;
using VehicleTaxonomy.Azure.Infrastructure.Files;

namespace VehicleTaxonomy.Azure.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptionsWithInjectionFactory<CosmosDbOptions>(configuration, CosmosDbOptions.SectionName)
            .AddOptionsWithInjectionFactory<BlobStorageOptions>(configuration, BlobStorageOptions.SectionName);

        services
            .AddSingleton<CosmosDbClientFactory>()
            .AddSingleton<BlobClientFactory>()
            .AddTransient<CosmosDbContainerInitializer>()
            .AddTransient<IVehicleTaxonomyRepository, VehicleTaxonomyRepository>()
            .AddTransient<CsvDataImportJobRunner>();

        return services;
    }
}
