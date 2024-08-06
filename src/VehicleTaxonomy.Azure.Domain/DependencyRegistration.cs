using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Domain.DataImport;
using VehicleTaxonomy.Azure.Domain.Makes;
using VehicleTaxonomy.Azure.Domain.Models;
using VehicleTaxonomy.Azure.Domain.Variants;
using VehicleTaxonomy.Azure.Infrastructure;

namespace VehicleTaxonomy.Azure.Domain;

public static class DependencyRegistration
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddTransient<ListMakesQueryHandler>()
            .AddTransient<IsMakeUniqueQueryHandler>()
            .AddTransient<AddMakeCommandHandler>()
            .AddTransient<DeleteMakeCommandHandler>()
            .AddTransient<ListModelsQueryHandler>()
            .AddTransient<IsModelUniqueQueryHandler>()
            .AddTransient<AddModelCommandHandler>()
            .AddTransient<DeleteModelCommandHandler>()
            .AddTransient<ListVariantsQueryHandler>()
            .AddTransient<IsVariantUniqueQueryHandler>()
            .AddTransient<AddVariantCommandHandler>()
            .AddTransient<DeleteVariantCommandHandler>()
            .AddTransient<ImportTaxonomyFromCsvCommandHandler>()
            .AddTransient<TaxonomyFromCsvImportJob>()
            ;

        return services;
    }
}
