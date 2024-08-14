using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Api;

public class ModelListApi
{
    private readonly ListModelsQueryHandler _listModelsQueryHandler;

    public ModelListApi(
        ListModelsQueryHandler listModelsQueryHandler
        )
    {
        _listModelsQueryHandler = listModelsQueryHandler;
    }

    [OpenApiOperation(
        nameof(ModelList),
        ModelApiConstants.CollectionTag,
        Description = "Lists all the models that belong to a specific make.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent make to filter models by e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiStandardResponseWithData<IReadOnlyCollection<Model>>(
        Description = "Returns a  collection of models, ordered by name.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(ModelList))]
    public async Task<IActionResult> ModelList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ModelApiConstants.RoutePrefix)] HttpRequest req,
        string makeId,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _listModelsQueryHandler.ExecuteAsync(new()
        {
            MakeId = makeId
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<IReadOnlyCollection<Model>>
    {
        public override IReadOnlyCollection<Model> Example => [
            new() {
                ModelId = "golf-plus",
                Name = "Golf Plus"
            },
            new() {
                ModelId = "polo",
                Name = "Polo"
            },
            new() {
                ModelId = "t-roc",
                Name = "T-Roc"
            }];
    }

}
