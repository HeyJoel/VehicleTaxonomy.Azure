using Newtonsoft.Json.Serialization;

namespace VehicleTaxonomy.Api;

/// <summary>
/// A basic example of a successful response when no data payload is required.
/// </summary>
public class OpenApiStandardSuccessResponseExample : OpenApiExample<ApiResponse<object>>
{
    public string Name { get; } = "Success";

    public override IOpenApiExample<ApiResponse<object>> Build(NamingStrategy? namingStrategy = null)
    {
        var example = new ApiResponse<object>()
        {
            IsValid = true,
            Result = null
        };

        Examples.Add(OpenApiExampleResolver.Resolve(Name, example, namingStrategy));

        return this;
    }
}
