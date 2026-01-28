using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Domain.Makes;

namespace VehicleTaxonomy.Domain.Tests.Makes.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class IsMakeUniqueQueryHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public IsMakeUniqueQueryHandlerTests(DbDependentFixture dbDependentFixture)
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenUnique_ReturnsTrue()
    {
        var name = ScopedString.FromMethodName(50);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new() { Name = name });

        Assert.True(result.IsValid);
        Assert.True(result.Result);
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
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();

        var result = await handler.ExecuteAsync(new() { Name = name! });

        Assert.False(result.IsValid);
        Assert.Single(result.ValidationErrors);

        var error = result.ValidationErrors.First();
        Assert.Equal(nameof(IsMakeUniqueQuery.Name), error.Property);
        Assert.False(result.Result);
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsFalse()
    {
        var name = ScopedString.FromMethodName(50);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IsMakeUniqueQueryHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        await makeTestHelper.AddMakeAsync(name);

        var result = await handler.ExecuteAsync(new() { Name = name });

        Assert.True(result.IsValid);
        Assert.False(result.Result);
    }
}
