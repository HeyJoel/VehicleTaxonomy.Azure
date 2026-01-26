using Newtonsoft.Json.Serialization;

namespace VehicleTaxonomy.Api;

public class OpenApiStandardValidationErrorResponseExample : OpenApiExample<ApiResponse<object>>
{
    public override IOpenApiExample<ApiResponse<object>> Build(NamingStrategy? namingStrategy = null)
    {
        var example = new ApiResponse<object>()
        {
            IsValid = false,
            ValidationErrors =
            [
                new() { Message = "The example property is required", Property = "ExampleProperty" },
                new() { Message = "Example validation error" }
            ]
        };

        Examples.Add(OpenApiExampleResolver.Resolve("Validation Error", example, namingStrategy));

        return this;
    }
}
