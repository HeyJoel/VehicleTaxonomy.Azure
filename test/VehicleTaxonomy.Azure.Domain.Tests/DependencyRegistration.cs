using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Domain.Tests.Makes;
using VehicleTaxonomy.Azure.Domain.Tests.Models;
using VehicleTaxonomy.Azure.Domain.Tests.Variants;

namespace VehicleTaxonomy.Azure.Domain;

public static class DependencyRegistration
{
    public static IServiceCollection AddDomainTests(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomain(configuration)
            .AddTransient<MakeTestHelper>()
            .AddTransient<ModelTestHelper>()
            .AddTransient<VariantTestHelper>()
            ;

        return services;
    }
}
