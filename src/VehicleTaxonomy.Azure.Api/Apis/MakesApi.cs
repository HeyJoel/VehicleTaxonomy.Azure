using VehicleTaxonomy.Azure.Domain.Makes;

namespace VehicleTaxonomy.Azure.Api;

public class MakesApi
{
    const string ROUTE_PREFIX = "makes";

    private readonly ListMakesQueryHandler _listMakesQueryHandler;
    private readonly IsMakeUniqueQueryHandler _isMakeUniqueQueryHandler;
    private readonly AddMakeCommandHandler _addMakeCommandHandler;
    private readonly DeleteMakeCommandHandler _deleteMakeCommandHandler;

    public MakesApi(
        ListMakesQueryHandler listMakesQueryHandler,
        IsMakeUniqueQueryHandler isMakeUniqueQueryHandler,
        AddMakeCommandHandler addMakeCommandHandler,
        DeleteMakeCommandHandler deleteMakeCommandHandler
        )
    {
        _listMakesQueryHandler = listMakesQueryHandler;
        _isMakeUniqueQueryHandler = isMakeUniqueQueryHandler;
        _addMakeCommandHandler = addMakeCommandHandler;
        _deleteMakeCommandHandler = deleteMakeCommandHandler;
    }

    [Function(nameof(MakeList))]
    public async Task<IActionResult> MakeList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX)] HttpRequest req,
        string? name,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _listMakesQueryHandler.ExecuteAsync(new()
        {
            Name = name
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    [Function(nameof(MakeIsUnique))]
    public async Task<IActionResult> MakeIsUnique(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ROUTE_PREFIX + "/is-unique")] HttpRequest req,
        string? name,
        CancellationToken cancellationToken
        )
    {
        var queryResponse = await _isMakeUniqueQueryHandler.ExecuteAsync(new()
        {
            Name = name ?? string.Empty
        }, cancellationToken);

        return ApiResponseHelper.ToResult(queryResponse);
    }

    [Function(nameof(MakeAdd))]
    public async Task<IActionResult> MakeAdd(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ROUTE_PREFIX)] HttpRequest req,
        [FromBody] AddMakeCommand command,
        CancellationToken cancellationToken
        )
    {
        var commandResponse = await _addMakeCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    [Function(nameof(MakeDelete))]
    public async Task<IActionResult> MakeDelete(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = ROUTE_PREFIX + "/{makeId}")] HttpRequest req,
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
