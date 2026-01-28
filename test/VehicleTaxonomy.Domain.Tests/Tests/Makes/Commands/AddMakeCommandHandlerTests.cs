using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Makes;

namespace VehicleTaxonomy.Domain.Tests.Makes.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddMakeCommandHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public AddMakeCommandHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValid_CanAdd()
    {
        var name = ScopedString.FromMethodName(50);
        var id = EntityIdFormatter.Format(name);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        var result = await handler.ExecuteAsync(new() { Name = name });

        var dbRecord = await makeTestHelper.GetRawDocumentAsync(id);

        Assert.True(result.IsValid);
        Assert.Equal(id, result.Result.Id);
        Assert.NotNull(dbRecord);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbRecord, """
                ParentPath: /
                PublicId: addmakech-whenvalid-canadd
                Name: AddMakeCH_WhenValid_CanAdd
                CreateDate: 2024-07-16T08:23:56
                """);
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
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();

        var result = await handler.ExecuteAsync(new() { Name = name! });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(AddMakeCommand.Name), error.Property);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        var name = ScopedString.FromMethodName(50);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddMakeCommandHandler>();

        var result1 = await handler.ExecuteAsync(new() { Name = name });
        var result2 = await handler.ExecuteAsync(new() { Name = name });

        Assert.True(result1.IsValid);
        Assert.False(result2.IsValid);
        Assert.Single(result2.ValidationErrors);

        var error = result2.ValidationErrors.First();
        Assert.Equal(nameof(AddMakeCommand.Name), error.Property);
        Assert.Contains("already exists", error.Message);
    }
}
