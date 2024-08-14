using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Api;

public class VariantListApi
{
    private readonly ListVariantsQueryHandler _listVariantsQueryHandler;

    public VariantListApi(
        ListVariantsQueryHandler listVariantsQueryHandler
        )
    {
        _listVariantsQueryHandler = listVariantsQueryHandler;
    }

    [OpenApiOperation(
        nameof(VariantList),
        VariantApiConstants.CollectionTag,
        Description = "Lists all the variants that belong to a specific model.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent make to filter variants by e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(modelId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "The unique id of the parent model that the variant belongs to e.g. \"polo\" or \"3-series\".",
        Example = typeof(ModelIdQueryParameterExample))]
    [OpenApiStandardResponseWithData<IReadOnlyCollection<Variant>>(
        Description = "Returns a collection of variants, ordered by name.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(VariantList))]
    public async Task<IActionResult> VariantList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = VariantApiConstants.RoutePrefix)] HttpRequest req,
        string makeId,
        string modelId,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _listVariantsQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<IReadOnlyCollection<Variant>>
    {
        public override IReadOnlyCollection<Variant> Example => [
            new() {
                VariantId = "polo-match-tdi-1-5l-deisel",
                Name = "Polo Match TDI 1.5l Diesel",
                EngineSizeInCC = 1500,
                FuelCategory = FuelCategory.Diesel
            },
            new() {
                VariantId = "polo-se-1-1l-petrol",
                Name = "Polo SE 1.1l Petrol",
                EngineSizeInCC = 1100,
                FuelCategory = FuelCategory.Petrol
            },
            new() {
                VariantId = "polo-sport-1-6-petrol",
                Name = "Polo Sport 1.6 Petrol",
                EngineSizeInCC = 1600,
                FuelCategory = FuelCategory.Petrol
            }];
    }

}
