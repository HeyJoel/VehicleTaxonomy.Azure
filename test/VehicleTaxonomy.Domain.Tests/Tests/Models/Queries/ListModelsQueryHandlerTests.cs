using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Models;
using VehicleTaxonomy.Infrastructure;

namespace VehicleTaxonomy.Domain.Tests.Models.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class ListModelsQueryHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public ListModelsQueryHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanReturnUnfiltered()
    {
        var uniqueData = ScopedString.FromMethodName(49);
        var name1 = uniqueData + "1";
        var name2 = name1 + "2";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListModelsQueryHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var make1Id = await modelTestHelper.AddMakeAsync(uniqueData);
        var make2Id = await modelTestHelper.AddMakeAsync(uniqueData + "ignored");
        await modelTestHelper.AddModelAsync(make1Id, name2);
        await modelTestHelper.AddModelAsync(make1Id, name1);
        await modelTestHelper.AddModelAsync(make2Id, name1);

        var response = await handler.ExecuteAsync(new() { MakeId = make1Id });

        Assert.True(response.IsValid);
        Assert.NotNull(response.Result);
        Assert.NotEmpty(response.Result);

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(name1))
            .ToArray();

        InlineSnapshot.Validate(filteredResults, """
            - ModelId: listmodelsqh-canreturnunfiltered1
              Name: ListModelsQH_CanReturnUnfiltered1
            - ModelId: listmodelsqh-canreturnunfiltered12
              Name: ListModelsQH_CanReturnUnfiltered12
            """);
    }
}
