using VehicleTaxonomy.Azure.Api.OpenApi;
using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Api;

public class VariantIsUniqueApi
{
    private readonly IsVariantUniqueQueryHandler _isVariantUniqueQueryHandler;

    public VariantIsUniqueApi(
        IsVariantUniqueQueryHandler isVariantUniqueQueryHandler
        )
    {
        _isVariantUniqueQueryHandler = isVariantUniqueQueryHandler;
    }

    [OpenApiOperation(
        nameof(VariantIsUnique),
        VariantApiConstants.CollectionTag,
        Description = "Determine if the name or name-derived id of a variant already exists for the specified make and model.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent make that the variant belongs to e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(modelId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent model that the variant belongs to e.g. \"polo\" or \"3-series\".",
        Example = typeof(ModelIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(name),
        In = ParameterLocation.Query,
        Required = true,
        Description = "Required. The name of the variant to check for uniqueness e.g. \"Polo\", \"305\" or \"Golf Plus\". The uniqueness check is based on the normalized version of the name which only includes letters (a-z), numbers and dashes, and is case-insensitive.",
        Example = typeof(NameParameterExample))]
    [OpenApiStandardResponseWithData<bool>(
        Description = "Returns true if the variant name and derived public id is unique to the parent make; otherwise false.",
        Example = typeof(OpenApiStandardIsUniqueExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(VariantIsUnique))]
    public async Task<IActionResult> VariantIsUnique(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = VariantApiConstants.RoutePrefix + "/is-unique")] HttpRequest req,
        string makeId,
        string modelId,
        string? name,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _isVariantUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name ?? string.Empty
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    public class NameParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "Polo SE 1.1l Petrol";
    }
}
