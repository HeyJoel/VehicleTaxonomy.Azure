using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Makes;
using VehicleTaxonomy.Infrastructure;

namespace VehicleTaxonomy.Domain.Tests.Makes.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class ListMakesQueryHandlerTests
{
    private const string UniquePrefix = "ListMakesQH_";
    private readonly DbDependentFixture _dbDependentFixture;

    public ListMakesQueryHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanReturnUnfiltered()
    {
        var name1 = ScopedString.FromMethodName(49);
        var name2 = name1 + "2";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListMakesQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        await makeTestHelper.AddMakeAsync(name2);
        await makeTestHelper.AddMakeAsync(name1);

        var response = await handler.ExecuteAsync(new());

        Assert.True(response.IsValid);
        Assert.NotNull(response.Result);
        Assert.NotEmpty(response.Result);

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(name1))
            .ToArray();

        InlineSnapshot.Validate(filteredResults, """
            - MakeId: listmakesqh-canreturnunfiltered
              Name: ListMakesQH_CanReturnUnfiltered
            - MakeId: listmakesqh-canreturnunfiltered2
              Name: ListMakesQH_CanReturnUnfiltered2
            """);
    }

    [Fact]
    public async Task CanFilterByName()
    {
        const string namePrefix = UniquePrefix + nameof(CanFilterByName);
        const string name1 = namePrefix + "1";
        const string name2 = namePrefix + "Two";
        const string name21 = namePrefix + "twoOne";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ListMakesQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        await makeTestHelper.AddMakeAsync(name1);
        await makeTestHelper.AddMakeAsync(name2);
        await makeTestHelper.AddMakeAsync(name21);

        var response = await handler.ExecuteAsync(new() { Name = name2 });


        Assert.True(response.IsValid);
        Assert.NotNull(response.Result);
        Assert.NotEmpty(response.Result);

        var filteredResults = EnumerableHelper
            .Enumerate(response.Result)
            .Where(r => r.Name.StartsWith(namePrefix))
            .ToArray();

        InlineSnapshot.Validate(filteredResults, """
            - MakeId: listmakesqh-canfilterbynametwo
              Name: ListMakesQH_CanFilterByNameTwo
            - MakeId: listmakesqh-canfilterbynametwoone
              Name: ListMakesQH_CanFilterByNametwoOne
            """);
    }
}
