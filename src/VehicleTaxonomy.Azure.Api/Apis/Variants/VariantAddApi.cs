using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Api;

public class VariantAddApi
{
    private readonly AddVariantCommandHandler _addVariantCommandHandler;

    public VariantAddApi(
        AddVariantCommandHandler addVariantCommandHandler
        )
    {
        _addVariantCommandHandler = addVariantCommandHandler;
    }

    [OpenApiOperation(
        nameof(VariantAdd),
        VariantApiConstants.CollectionTag,
        Description = "Create a new variant. If a variant with the same name, make and model already exists then a validation error is returned.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Required. The unique id of the parent make that the model associated with modelId belongs to e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiParameter(
        nameof(modelId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "The unique id of the parent model that the variant belongs to e.g. \"polo\" or \"3-series\".",
        Example = typeof(ModelIdQueryParameterExample))]
    [OpenApiStandardCommandRequestBody<AddVariantCommand>(
        Example = typeof(CommandExample))]
    [OpenApiStandardResponseWithData<AddEntityResult>(
        Description = "Returns the unique id of the newly created variant.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(VariantAdd))]
    public async Task<IActionResult> VariantAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = VariantApiConstants.RoutePrefix)] HttpRequest req,
        string makeId,
        string modelId,
        AddVariantCommand command,
        CancellationToken cancellationToken
        )
    {
        command.MakeId = makeId;
        command.ModelId = modelId;
        var commandResponse = await _addVariantCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    public class CommandExample : OpenApiStandardExampleBase<AddVariantCommand>
    {
        public override AddVariantCommand Example => new()
        {
            MakeId = "volkswagen",
            ModelId = "polo",
            Name = "Polo SE 1.1l Petrol",
            EngineSizeInCC = 1100,
            FuelCategory = FuelCategory.Petrol
        };
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<AddEntityResult>
    {
        public override AddEntityResult Example => new()
        {
            Id = "polo-se-1-1l-petrol"
        };
    }
}
