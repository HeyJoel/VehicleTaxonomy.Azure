using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.DataImport;
using VehicleTaxonomy.Domain.Makes;
using VehicleTaxonomy.Domain.Models;
using VehicleTaxonomy.Domain.Variants;
using VehicleTaxonomy.Infrastructure;

namespace VehicleTaxonomy.Domain;

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
            .AddTransient<TaxonomyFromCsvImportJob>();

        return services;
    }
}
