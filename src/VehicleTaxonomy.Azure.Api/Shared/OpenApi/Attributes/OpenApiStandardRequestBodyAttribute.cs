namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Encapsulates <see cref="OpenApiRequestBodyAttribute"/> providing
/// sensible defaults for standard request parameters wrapped in a
/// command object.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OpenApiStandardCommandRequestBodyAttribute<TCommand> : OpenApiRequestBodyAttribute
{
    public OpenApiStandardCommandRequestBodyAttribute()
        : base("application/json", typeof(TCommand))
    {
        Description = "Command parameters.";
    }
}
