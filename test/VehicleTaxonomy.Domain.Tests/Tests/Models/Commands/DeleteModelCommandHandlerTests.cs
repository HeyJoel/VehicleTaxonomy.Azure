using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Models;

namespace VehicleTaxonomy.Domain.Tests.Models.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class DeleteModelCommandHandlerTests
{
    private const string UniquePrefix = "DelModelCH_";
    private readonly DbDependentFixture _dbDependentFixture;

    public DeleteModelCommandHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanDelete()
    {
        const string uniqueData = UniquePrefix + nameof(CanDelete);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();

        var makeId = await modelTestHelper.AddMakeAsync(uniqueData);
        var id = await modelTestHelper.AddModelAsync(makeId, uniqueData);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId,
            ModelId = id
        });

        var dbRecord = await modelTestHelper.GetRawDocumentAsync(makeId, id);

        Assert.True(result.IsValid);
        Assert.Null(dbRecord);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenMakeIdInvalid_ReturnsError(string? makeId)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = makeId!,
            ModelId = "na"
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(DeleteModelCommand.MakeId), error.Property);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("!!!")]
    [InlineData("aBc")]
    [InlineData("lorem-ipsum-dolor-amet-con-sectetur-adipiscing-elit")]
    public async Task WhenIdInvalid_ReturnsError(string? id)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = "na",
            ModelId = id!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(DeleteModelCommand.ModelId), error.Property);
    }

    [Fact]
    public async Task WhenNotExists_ReturnsError()
    {
        const string uniqueData = UniquePrefix + nameof(WhenNotExists_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(uniqueData),
            ModelId = EntityIdFormatter.Format(uniqueData)
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(DeleteModelCommand.ModelId), error.Property);
        Assert.Contains("not be found", error.Message);
    }
}
