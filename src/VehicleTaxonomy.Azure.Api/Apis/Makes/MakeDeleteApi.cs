using VehicleTaxonomy.Azure.Domain.Makes;

namespace VehicleTaxonomy.Azure.Api;

public class MakeDeleteApi
{
    private readonly DeleteMakeCommandHandler _deleteMakeCommandHandler;

    public MakeDeleteApi(
        DeleteMakeCommandHandler deleteMakeCommandHandler
        )
    {
        _deleteMakeCommandHandler = deleteMakeCommandHandler;
    }

    [OpenApiOperation(
        nameof(MakeDelete),
        MakeApiConstants.CollectionTag,
        Description = "Delete an existing make. If the make does not exist then a validation error is returned.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Id of the make to delete e.g. \"volkswagen\", \"mg\" or \"mercedes-benz\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiStandardSuccessResponse]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(MakeDelete))]
    public async Task<IActionResult> MakeDelete(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = MakeApiConstants.RoutePrefix + "/{makeId}")] HttpRequest req,
        string makeId,
        CancellationToken cancellationToken
        )
    {
        var commandResponse = await _deleteMakeCommandHandler.ExecuteAsync(new()
        {
            MakeId = makeId
        }, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }
}
