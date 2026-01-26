using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Makes;

namespace VehicleTaxonomy.Domain.Tests.Makes.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class DeleteMakeCommandHandlerTests
{
    private const string UniquePrefix = "DelMakeCH_";
    private readonly DbDependentFixture _dbDependentFixture;

    public DeleteMakeCommandHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task CanDelete()
    {
        const string name = UniquePrefix + nameof(CanDelete);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        var id = await makeTestHelper.AddMakeAsync(name);

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id
        });

        var dbRecord = await makeTestHelper.GetRawDocumentAsync(id);

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
    public async Task WhenIdInvalid_ReturnsError(string? name)
    {
        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = name!
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);
        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(DeleteMakeCommand.MakeId), error.Property);
    }

    [Fact]
    public async Task WhenNotExists_ReturnsError()
    {
        const string name = UniquePrefix + nameof(WhenNotExists_ReturnsError);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<DeleteMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = EntityIdFormatter.Format(name)
        });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(DeleteMakeCommand.MakeId), error.Property);
        Assert.Contains("not be found", error.Message);
    }
}
