using System.Net;

namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Encapsulates <see cref="OpenApiResponseWithBodyAttribute"/> providing
/// sensible defaults for standard responses.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OpenApiStandardSuccessResponseAttribute : OpenApiResponseWithBodyAttribute
{
    public OpenApiStandardSuccessResponseAttribute()
        : base(HttpStatusCode.OK, "application/json", typeof(ApiResponse<object>))
    {
        Description = "Standard successful response";
        Example = typeof(OpenApiStandardSuccessResponseExample);
    }
}
