using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Variants;

namespace VehicleTaxonomy.Domain.Tests.Variants.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddVariantCommandHandlerTests
{
    private const string UniquePrefix = "AddVariantCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public AddVariantCommandHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValid_CanAdd()
    {
        const string uniqueData = UniquePrefix + nameof(WhenValid_CanAdd);
        const string name = uniqueData;
        const string id = "addvariantch-whenvalid-canadd";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        var dbRecord = await variantTestHelper.GetRawDocumentAsync(makeId, modelId, id);

        Assert.True(result.IsValid);
        Assert.Equal(id, result.Result.Id);
        Assert.NotNull(dbRecord);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbRecord, """
                EntityType: Variant
                ParentPath: /addvariantch-whenvalid-canaddmk/addvariantch-whenvalid-canaddmd
                PublicId: addvariantch-whenvalid-canadd
                Name: AddVariantCH_WhenValid_CanAdd
                CreateDate: 2024-07-16T08:23:56
                VariantData: {}
                """);
    }

    [Fact]
    public async Task CanAddWithOptionalProperties()
    {
        const string uniqueData = UniquePrefix + nameof(CanAddWithOptionalProperties);
        const string name = uniqueData;
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(uniqueData);
        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name,
            EngineSizeInCC = 4300,
            FuelCategory = FuelCategory.Petrol
        });

        var dbRecord = await variantTestHelper.GetRawDocumentAsync(makeId, modelId, id);

        Assert.True(result.IsValid);
        Assert.NotNull(dbRecord);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbRecord, """
                EntityType: Variant
                ParentPath: /addvariantch-canaddwithoptionalpropertiesmk/addvariantch-canaddwithoptionalpropertiesmd
                PublicId: addvariantch-canaddwithoptionalproperties
                Name: AddVariantCH_CanAddWithOptionalProperties
                CreateDate: 2024-07-16T08:23:56
                VariantData:
                  FuelCategory: Petrol
                  EngineSizeInCC: 4300
                """);
    }

    [Fact]
    public async Task WhenModelNotExists_ReturnsError()
    {
        const string uniqueData = UniquePrefix + nameof(WhenModelNotExists_ReturnsError);
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            ModelId = id,
            Name = id
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddVariantCommand.ModelId), error.Property);
        Assert.Contains("Model does not exist", error.Message);
        Assert.Null(result.Result);
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
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = id!,
            Name = "na"
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddVariantCommand.ModelId), error.Property);
        Assert.Null(result.Result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("Lorem ipsum dolor amet, consectetur adipiscing elit lorem ipsum dolor amet, consectetur adipiscing el")]
    public async Task WhenNameInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = "na",
            Name = name!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddVariantCommand.Name), error.Property);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        const string uniqueData = UniquePrefix + nameof(WhenNameNotUnique_ReturnsError);
        const string name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var (makeId, modelId) = await variantTestHelper.AddModelWithMakeAsync(uniqueData + "mk");

        var result1 = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        var result2 = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        });

        Assert.True(result1.IsValid);
        Assert.False(result2.IsValid);
        Assert.Single(result2.ValidationErrors);

        var error = result2.ValidationErrors.First();
        Assert.Equal(nameof(AddVariantCommand.Name), error.Property);
        Assert.Contains("already exists", error.Message);
    }
}
