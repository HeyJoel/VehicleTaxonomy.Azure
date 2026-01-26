using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Tests.Makes;
using VehicleTaxonomy.Domain.Tests.Models;
using VehicleTaxonomy.Domain.Tests.Variants;

namespace VehicleTaxonomy.Domain.Tests;

public static class DependencyRegistration
{
    public static IServiceCollection AddDomainTests(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomain(configuration)
            .AddTransient<MakeTestHelper>()
            .AddTransient<ModelTestHelper>()
            .AddTransient<VariantTestHelper>();

        return services;
    }
}
