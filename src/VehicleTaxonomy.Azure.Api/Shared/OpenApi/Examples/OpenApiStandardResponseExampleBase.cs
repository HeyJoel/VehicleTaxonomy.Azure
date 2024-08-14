using Newtonsoft.Json.Serialization;

namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Base class to help reduce some of the boilerplate with defining response examples.
/// </summary>
public abstract class OpenApiStandardResponseExampleBase<TData> : OpenApiExample<ApiResponse<TData>>
{
    public virtual string Name { get; } = "Success";

    public abstract TData Example { get; }

    public override IOpenApiExample<ApiResponse<TData>> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(
            OpenApiExampleResolver.Resolve(
                Name,
                new ApiResponse<TData>()
                {
                    IsValid = true,
                    Result = Example
                },
                namingStrategy
            ));

        return this;
    }
}
