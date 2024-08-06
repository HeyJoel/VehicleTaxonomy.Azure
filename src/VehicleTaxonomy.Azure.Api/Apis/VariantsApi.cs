using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Api;

public class VariantsApi
{
    const string ROUTE_PREFIX = "makes/{makeId}/models/{modelId}/variants";

    private readonly ListVariantsQueryHandler _listVariantsQueryHandler;
    private readonly IsVariantUniqueQueryHandler _isVariantUniqueQueryHandler;
    private readonly AddVariantCommandHandler _addVariantCommandHandler;
    private readonly DeleteVariantCommandHandler _deleteVariantCommandHandler;

    public VariantsApi(
        ListVariantsQueryHandler listVariantsQueryHandler,
        IsVariantUniqueQueryHandler isVariantUniqueQueryHandler,
        AddVariantCommandHandler addVariantCommandHandler,
        DeleteVariantCommandHandler deleteVariantCommandHandler
        )
    {
        _listVariantsQueryHandler = listVariantsQueryHandler;
        _isVariantUniqueQueryHandler = isVariantUniqueQueryHandler;
        _addVariantCommandHandler = addVariantCommandHandler;
        _deleteVariantCommandHandler = deleteVariantCommandHandler;
    }

    [Function(nameof(VariantList))]
    public async Task<IActionResult> VariantList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX)] HttpRequest req,
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

    [Function(nameof(VariantIsUnique))]
    public async Task<IActionResult> VariantIsUnique(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX + "/is-unique")] HttpRequest req,
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

    [Function(nameof(VariantAdd))]
    public async Task<IActionResult> VariantAdd(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ROUTE_PREFIX)] HttpRequest req,
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

    [Function(nameof(VariantDelete))]
    public async Task<IActionResult> VariantDelete(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = ROUTE_PREFIX + "/{variantId}")] HttpRequest req,
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
}
