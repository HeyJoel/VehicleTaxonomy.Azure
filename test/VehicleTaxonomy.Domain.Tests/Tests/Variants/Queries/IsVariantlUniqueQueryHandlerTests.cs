using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Variants;

namespace VehicleTaxonomy.Domain.Tests.Variants.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsVariantUniqueQueryHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public IsVariantUniqueQueryHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenUnique_ReturnsTrue()
    {
        var name = ScopedString.FromMethodName(48);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();
        var (make1Id, model1Id) = await variantTestHelper.AddModelWithMakeAsync(name);
        var (make2Id, model2Id) = await variantTestHelper.AddModelWithMakeAsync(name + "ignored");
        await variantTestHelper.AddVariantAsync(make1Id, model1Id, name + "ignored");
        await variantTestHelper.AddVariantAsync(make2Id, model2Id, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = make1Id,
            ModelId = model1Id,
            Name = name
        });

        Assert.True(result.IsValid);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task WhenModelNotExists_ReturnsFalse()
    {
        var uniqueData = ScopedString.FromMethodName(50);
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            ModelId = id,
            Name = uniqueData
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsVariantUniqueQuery.ModelId), error.Property);
        Assert.Contains("Model does not exist", error.Message);
        Assert.False(result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenModelIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = id!,
            Name = "na"
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsVariantUniqueQuery.ModelId), error.Property);
        Assert.False(result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData(
        "Lorem ipsum dolor amet, consectetur adipiscing elit Lorem ipsum dolor amet, consectetur adipiscing el")]
    public async Task WhenNameInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = "na",
            Name = name!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsVariantUniqueQuery.Name), error.Property);
        Assert.False(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        var name = ScopedString.FromMethodName(48);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsVariantUniqueQueryHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(name);
        await variantTestHelper.AddVariantAsync(makeId, modelId, name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        Assert.True(result.IsValid);
        Assert.False(result.Result);
    }
}
