using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Models;

namespace VehicleTaxonomy.Domain.Tests.Models.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddModelCommandHandlerTests
{
    private const string UniquePrefix = "AddModelCH_";
    private readonly DbDependentFixture _dbDependentFixture;

    public AddModelCommandHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValid_CanAdd()
    {
        const string uniqueData = UniquePrefix + nameof(WhenValid_CanAdd);
        const string name = uniqueData;
        const string id = "addmodelch-whenvalid-canadd";

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData + "mk");
        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        var dbRecord = await modelTestHelper.GetRawDocumentAsync(makeId, id);

        Assert.True(result.IsValid);
        Assert.Equal(id, result.Result.Id);
        Assert.NotNull(dbRecord);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbRecord, """
                EntityType: Model
                ParentPath: /addmodelch-whenvalid-canaddmk
                PublicId: addmodelch-whenvalid-canadd
                Name: AddModelCH_WhenValid_CanAdd
                CreateDate: 2024-07-16T08:23:56
                """);
    }

    [Fact]
    public async Task WhenMakeNotExists_ReturnsError()
    {
        const string uniqueData = UniquePrefix + nameof(WhenMakeNotExists_ReturnsError);
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            Name = id
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddModelCommand.MakeId), error.Property);
        Assert.Contains("Make does not exist", error.Message);
        Assert.Null(result.Result);
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
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id!,
            Name = "na"
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddModelCommand.MakeId), error.Property);
        Assert.Null(result.Result);
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
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            Name = name!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddModelCommand.Name), error.Property);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        const string uniqueData = UniquePrefix + nameof(WhenNameNotUnique_ReturnsError);
        const string name = uniqueData;

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData + "mk");

        var result1 = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        var result2 = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        Assert.True(result1.IsValid);
        Assert.False(result2.IsValid);
        Assert.Single(result2.ValidationErrors);

        var error = result2.ValidationErrors.First();
        Assert.Equal(nameof(AddModelCommand.Name), error.Property);
        Assert.Contains("already exists", error.Message);
    }
}
