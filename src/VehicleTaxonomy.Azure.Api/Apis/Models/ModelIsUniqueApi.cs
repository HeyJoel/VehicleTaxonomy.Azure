using VehicleTaxonomy.Azure.Api.OpenApi;
using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Api;

public class ModelIsUniqueApi
{
    private readonly IsModelUniqueQueryHandler _isModelUniqueQueryHandler;

    public ModelIsUniqueApi(
        IsModelUniqueQueryHandler isModelUniqueQueryHandler
        )
    {
        _isModelUniqueQueryHandler = isModelUniqueQueryHandler;
    }

    [OpenApiOperation(
        nameof(ModelIsUnique),
        ModelApiConstants.CollectionTag,
        Description = "Determine if the name or name-derived id of a model already exists for the specified make.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent make that the model belongs to e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(name),
        In = ParameterLocation.Query,
        Required = true,
        Description = "Required. The name of the model to check for uniqueness e.g. \"Polo\", \"305\" or \"Golf Plus\". The uniqueness check is based on the normalized version of the name which only includes letters (a-z), numbers and dashes, and is case-insensitive.",
        Example = typeof(NameParameterExample))]
    [OpenApiStandardResponseWithData<bool>(
        Description = "Returns true if the model name and derived public id is unique to the parent make; otherwise false.",
        Example = typeof(OpenApiStandardIsUniqueExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(ModelIsUnique))]
    public async Task<IActionResult> ModelIsUnique(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ModelApiConstants.RoutePrefix + "/is-unique")] HttpRequest req,
        string makeId,
        string? name,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _isModelUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name ?? string.Empty
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    public class NameParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "Polo";
    }
}
