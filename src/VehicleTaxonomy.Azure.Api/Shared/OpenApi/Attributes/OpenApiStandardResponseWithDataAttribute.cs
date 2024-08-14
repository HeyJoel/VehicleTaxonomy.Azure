using System.Net;

namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Encapsulates <see cref="OpenApiResponseWithBodyAttribute"/> providing
/// sensible defaults for standard responses.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OpenApiStandardResponseWithDataAttribute<TData> : OpenApiResponseWithBodyAttribute
{
    public OpenApiStandardResponseWithDataAttribute()
        : base(HttpStatusCode.OK, "application/json", typeof(ApiResponse<TData>))
    {
        Description = "Standard successful response";
    }
}
