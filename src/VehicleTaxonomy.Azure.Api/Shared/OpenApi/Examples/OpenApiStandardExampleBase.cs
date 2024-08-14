using Newtonsoft.Json.Serialization;

namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Base class to help reduce some of the boilerplate with defining examples.
/// </summary>
public abstract class OpenApiStandardExampleBase<TData> : OpenApiExample<TData>
{
    public virtual string Name { get; } = "Example";

    public abstract TData Example { get; }

    public override IOpenApiExample<TData> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(
            OpenApiExampleResolver.Resolve(
                Name,
                Example,
                namingStrategy
            ));

        return this;
    }
}
