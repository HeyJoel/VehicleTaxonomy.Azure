using VehicleTaxonomy.Azure.Domain.Makes;

namespace VehicleTaxonomy.Azure.Api;

public class MakeAddApi
{
    private readonly AddMakeCommandHandler _addMakeCommandHandler;

    public MakeAddApi(
        AddMakeCommandHandler addMakeCommandHandler
        )
    {
        _addMakeCommandHandler = addMakeCommandHandler;
    }

    [OpenApiOperation(
        nameof(MakeAdd),
        MakeApiConstants.CollectionTag,
        Description = "Create a new make. If a make with the same name already exists then a validation error is returned.")]
    [OpenApiStandardCommandRequestBody<AddMakeCommand>(
        Example = typeof(CommandExample))]
    [OpenApiStandardResponseWithData<AddEntityResult>(
        Description = "Returns the unique id of the newly created make.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(MakeAdd))]
    public async Task<IActionResult> MakeAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = MakeApiConstants.RoutePrefix)] HttpRequest req,
        [FromBody] AddMakeCommand command,
        CancellationToken cancellationToken
        )
    {
        var commandResponse = await _addMakeCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    public class CommandExample : OpenApiStandardExampleBase<AddMakeCommand>
    {
        public override AddMakeCommand Example => new()
        {
            Name = "Volkswagen"
        };
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<AddEntityResult>
    {
        public override AddEntityResult Example => new()
        {
            Id = "volkswagen"
        };
    }
}
