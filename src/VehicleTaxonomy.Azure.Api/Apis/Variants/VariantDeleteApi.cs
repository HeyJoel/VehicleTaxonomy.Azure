using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Api;

public class VariantDeleteApi
{
    private readonly DeleteVariantCommandHandler _deleteVariantCommandHandler;

    public VariantDeleteApi(
        DeleteVariantCommandHandler deleteVariantCommandHandler
        )
    {
        _deleteVariantCommandHandler = deleteVariantCommandHandler;
    }

    [OpenApiOperation(
        nameof(VariantDelete),
        VariantApiConstants.CollectionTag,
        Description = "Delete an existing variant. If the variant does not exist then a validation error is returned.")]
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
        Description = "The unique id of the parent model that the variant belongs to e.g. \"polo\" or \"3-series\".",
        Example = typeof(ModelIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(variantId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Id of the variant to delete e.g. \"3008-access-1-6l-petrol\" or \"id3-city-battery-electric\".",
        Example = typeof(VariantIdQueryParameterExample))]
    [OpenApiStandardSuccessResponse]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(VariantDelete))]
    public async Task<IActionResult> VariantDelete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = VariantApiConstants.RoutePrefix + "/{variantId}")] HttpRequest req,
        string makeId,
        string modelId,
        string variantId,
        CancellationToken cancellationToken
        )
    {
        var commandResponse = await _deleteVariantCommandHandler.ExecuteAsync(new()
        {
            MakeId = variantId,
            ModelId = modelId,
            VariantId = variantId
        }, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    public class VariantIdQueryParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "polo-se-1-1l-petrol";
    }
}
