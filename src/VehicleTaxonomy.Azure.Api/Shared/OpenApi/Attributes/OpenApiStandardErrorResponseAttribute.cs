using System.Net;

namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Encapsulates <see cref="OpenApiResponseWithBodyAttribute"/> providing
/// sensible defaults for the standard validation error responses.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OpenApiStandardValidationErrorResponseAttribute : OpenApiResponseWithBodyAttribute
{
    public OpenApiStandardValidationErrorResponseAttribute()
        : base(HttpStatusCode.BadRequest, "application/json", typeof(ApiResponse<object>))
    {
        Description = "Standard validation error response";
        Example = typeof(OpenApiStandardValidationErrorResponseExample);
    }
}
