using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Api;

public class ModelAddApi
{
    private readonly AddModelCommandHandler _addModelCommandHandler;

    public ModelAddApi(
        AddModelCommandHandler addModelCommandHandler
        )
    {
        _addModelCommandHandler = addModelCommandHandler;
    }

    [OpenApiOperation(
        nameof(ModelAdd),
        ModelApiConstants.CollectionTag,
        Description = "Create a new model. If a model with the same name and make already exists then a validation error is returned.")]
    [OpenApiParameter(
        nameof(makeId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "The unique id of the parent make that the model belongs to e.g. \"volkswagen\" or \"bmw\".",
        Example = typeof(MakeIdQueryParameterExample))]
    [OpenApiStandardCommandRequestBody<AddModelCommand>(
        Example = typeof(CommandExample))]
    [OpenApiStandardResponseWithData<AddEntityResult>(
        Description = "Returns the unique id of the newly created model.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(ModelAdd))]
    public async Task<IActionResult> ModelAdd(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ModelApiConstants.RoutePrefix)] HttpRequest req,
        string makeId,
        [FromBody] AddModelCommand command,
        CancellationToken cancellationToken
        )
    {
        command.MakeId = makeId;
        var commandResponse = await _addModelCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    public class CommandExample : OpenApiStandardExampleBase<AddModelCommand>
    {
        public override AddModelCommand Example => new()
        {
            MakeId = "volkswagen",
            Name = "Polo"
        };
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<AddEntityResult>
    {
        public override AddEntityResult Example => new()
        {
            Id = "polo"
        };
    }
}
