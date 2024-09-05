using VehicleTaxonomy.Azure.Domain.Makes;

namespace VehicleTaxonomy.Azure.Api;

public class MakeListApi
{
    private readonly ListMakesQueryHandler _listMakesQueryHandler;

    public MakeListApi(
        ListMakesQueryHandler listMakesQueryHandler
        )
    {
        _listMakesQueryHandler = listMakesQueryHandler;
    }

    [OpenApiOperation(
        nameof(MakeList),
        MakeApiConstants.CollectionTag,
        Description = "Lists all makes, optionally filtered by name.")]
    [OpenApiParameter(
        nameof(name),
        In = ParameterLocation.Query,
        Description = "Optionally filter by the make name, using a case-insensitive \"starts-with\" comparision.")]
    [OpenApiStandardResponseWithData<IReadOnlyCollection<Make>>(
        Description = "Returns a  collection of makes, ordered by name.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(MakeList))]
    public async Task<IActionResult> MakeList(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = MakeApiConstants.RoutePrefix)] HttpRequest req,
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

    public class ResultExample : OpenApiStandardResponseExampleBase<IReadOnlyCollection<Make>>
    {
        public override IReadOnlyCollection<Make> Example => [
            new() {
                MakeId = "alpha-romeo",
                Name = "Alpha Romeo"
            },
            new() {
                MakeId = "audi",
                Name = "Audi"
            },
            new() {
                MakeId = "bentley",
                Name = "Bentley"
            }];
    }

}
