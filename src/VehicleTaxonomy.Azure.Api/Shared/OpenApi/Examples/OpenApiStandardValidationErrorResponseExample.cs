using Newtonsoft.Json.Serialization;

namespace VehicleTaxonomy.Azure.Api;

public class OpenApiStandardValidationErrorResponseExample : OpenApiExample<ApiResponse<object>>
{
    public override IOpenApiExample<ApiResponse<object>> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(
            OpenApiExampleResolver.Resolve(
                "Validation Error",
                new ApiResponse<object>()
                {
                    IsValid = false,
                    ValidationErrors = [
                        new() {
                                Message = "The example property is required",
                                Property = "ExampleProperty"
                        },
                        new() {
                            Message = "Example validation error"
                        }]

                },
                namingStrategy
            ));

        return this;
    }
}
