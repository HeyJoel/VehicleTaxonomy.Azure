using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;

namespace VehicleTaxonomy.Api;

public static class OpenApiConfiguration
{
    public static IOpenApiConfigurationOptions GetConfiguration()
    {
        var options = new OpenApiConfigurationOptions()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0",
                Title = "VehicleTaxonomy.Api",
                Description = "An example of a microservice that can be used to classify vehicles by make, model and variant.",
                Contact = new OpenApiContact()
                {
                    Name = "GitHub",
                    Url = new Uri("https://github.com/HeyJoel/VehicleTaxonomy/"),
                },
                License = new OpenApiLicense()
                {
                    Name = "MIT",
                    Url = new Uri("https://github.com/HeyJoel/VehicleTaxonomy/blob/main/LICENSE"),
                }
            },
            Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
            OpenApiVersion = OpenApiVersionType.V3,
            IncludeRequestingHostName = true
        };

        return options;
    }
}
