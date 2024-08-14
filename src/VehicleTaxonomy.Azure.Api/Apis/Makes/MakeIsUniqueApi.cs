using VehicleTaxonomy.Azure.Api.OpenApi;
using VehicleTaxonomy.Azure.Domain.Makes;

namespace VehicleTaxonomy.Azure.Api;

public class MakeIsUniqueApi
{
    private readonly IsMakeUniqueQueryHandler _isMakeUniqueQueryHandler;

    public MakeIsUniqueApi(
        IsMakeUniqueQueryHandler isMakeUniqueQueryHandler
        )
    {
        _isMakeUniqueQueryHandler = isMakeUniqueQueryHandler;
    }

    [OpenApiOperation(
        nameof(MakeIsUnique),
        MakeApiConstants.CollectionTag,
        Description = "Determine if the name or name-derived id of a make already exists in the system.")]
    [OpenApiParameter(
        nameof(name),
        In = ParameterLocation.Query,
        Required = true,
        Description = "Required. The name of the make to check for uniqueness e.g. \"Volkswagen\", \"MG\" or \"Mercedes Benz\". The uniqueness check is based on the normalized version of the name which only includes letters (a-z), numbers and dashes, and is case-insensitive.",
        Example = typeof(NameParameterExample))]
    [OpenApiStandardResponseWithData<bool>(
        Description = "Returns true if the make name and derived public id is unique; otherwise false.",
        Example = typeof(OpenApiStandardIsUniqueExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(MakeIsUnique))]
    public async Task<IActionResult> MakeIsUnique(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = MakeApiConstants.RoutePrefix + "/is-unique")] HttpRequest req,
        string? name,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _isMakeUniqueQueryHandler.ExecuteAsync(new()
        {
            Name = name ?? string.Empty
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    public class NameParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "Volkswagen";
    }
}
