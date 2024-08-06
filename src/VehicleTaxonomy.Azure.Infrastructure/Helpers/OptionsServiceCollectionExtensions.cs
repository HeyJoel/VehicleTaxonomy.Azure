using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VehicleTaxonomy.Azure.Infrastructure;

public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsWithInjectionFactory<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName
        )
        where TOptions : class
    {
        var section = configuration.GetSection(sectionName);

        services
            .AddOptions<TOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(s => s.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
}
