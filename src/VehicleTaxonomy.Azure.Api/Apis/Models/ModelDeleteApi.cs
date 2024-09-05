using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Api;

public class ModelDeleteApi
{
    private readonly DeleteModelCommandHandler _deleteModelCommandHandler;

    public ModelDeleteApi(
        DeleteModelCommandHandler deleteModelCommandHandler
        )
    {
        _deleteModelCommandHandler = deleteModelCommandHandler;
    }

    [OpenApiOperation(
        nameof(ModelDelete),
        ModelApiConstants.CollectionTag,
        Description = "Delete an existing model. If the model does not exist then a validation error is returned.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "The unique id of the parent make that the model belongs to e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(modelId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Id of the make to delete e.g. \"polo\", \"305\" or \"3-series\".",
        Example = typeof(ModelIdQueryParameterExample))]
    [OpenApiStandardSuccessResponse]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(ModelDelete))]
    public async Task<IActionResult> ModelDelete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = ModelApiConstants.RoutePrefix + "/{modelId}")] HttpRequest req,
        string makeId,
        string modelId,
        CancellationToken cancellationToken
        )
    {
        var commandResponse = await _deleteModelCommandHandler.ExecuteAsync(new()
        {
            MakeId = modelId,
            ModelId = modelId
        }, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }
}
