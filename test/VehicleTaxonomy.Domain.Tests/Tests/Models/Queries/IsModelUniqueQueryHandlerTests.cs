using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Models;

namespace VehicleTaxonomy.Domain.Tests.Models.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsModelUniqueQueryHandlerTests
{
    private const string UniquePrefix = "IsModelUniqueQH_";
    private readonly DbDependentFixture _dbDependentFixture;

    public IsModelUniqueQueryHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenUnique_ReturnsTrue()
    {
        const string uniqueData = UniquePrefix + nameof(WhenUnique_ReturnsTrue);
        var name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();
        var make1Id = await modelTestHelper.AddMakeAsync(uniqueData);
        var make2Id = await modelTestHelper.AddMakeAsync(uniqueData + "ignored");
        await modelTestHelper.AddModelAsync(make1Id, name + "ignored");
        await modelTestHelper.AddModelAsync(make2Id, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = make1Id,
            Name = name
        });

        Assert.True(result.IsValid);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task WhenMakeNotExists_ReturnsFalse()
    {
        const string uniqueData = UniquePrefix + nameof(WhenMakeNotExists_ReturnsFalse);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(uniqueData),
            Name = uniqueData
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsModelUniqueQuery.MakeId), error.Property);
        Assert.Contains("Make does not exist", error.Message);
        Assert.False(result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenMakeIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id!,
            Name = "na"
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsModelUniqueQuery.MakeId), error.Property);
        Assert.False(result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("Lorem ipsum dolor amet, consectetur adipiscing elit")]
    public async Task WhenNameInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            Name = name!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsModelUniqueQuery.Name), error.Property);
        Assert.False(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        const string uniqueData = UniquePrefix + nameof(WhenNameNotUnique_ReturnsFalse);
        const string name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsModelUniqueQueryHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData);
        await modelTestHelper.AddModelAsync(makeId, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        Assert.True(result.IsValid);
        Assert.False(result.Result);
    }
}
