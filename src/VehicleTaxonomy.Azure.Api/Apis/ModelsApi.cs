using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Api;

public class ModelsApi
{
    const string ROUTE_PREFIX = "makes/{makeId}/models";

    private readonly ListModelsQueryHandler _listModelsQueryHandler;
    private readonly IsModelUniqueQueryHandler _isModelUniqueQueryHandler;
    private readonly AddModelCommandHandler _addModelCommandHandler;
    private readonly DeleteModelCommandHandler _deleteModelCommandHandler;

    public ModelsApi(
        ListModelsQueryHandler listModelsQueryHandler,
        IsModelUniqueQueryHandler isModelUniqueQueryHandler,
        AddModelCommandHandler addModelCommandHandler,
        DeleteModelCommandHandler deleteModelCommandHandler
        )
    {
        _listModelsQueryHandler = listModelsQueryHandler;
        _isModelUniqueQueryHandler = isModelUniqueQueryHandler;
        _addModelCommandHandler = addModelCommandHandler;
        _deleteModelCommandHandler = deleteModelCommandHandler;
    }

    [Function(nameof(ModelList))]
    public async Task<IActionResult> ModelList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX)] HttpRequest req,
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

    [Function(nameof(ModelIsUnique))]
    public async Task<IActionResult> ModelIsUnique(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX + "/is-unique")] HttpRequest req,
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

    [Function(nameof(ModelAdd))]
    public async Task<IActionResult> ModelAdd(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ROUTE_PREFIX)] HttpRequest req,
        string makeId,
        [FromBody] AddModelCommand command,
        CancellationToken cancellationToken
        )
    {
        command.MakeId = makeId;
        var commandResponse = await _addModelCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    [Function(nameof(ModelDelete))]
    public async Task<IActionResult> ModelDelete(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = ROUTE_PREFIX + "/{modelId}")] HttpRequest req,
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
