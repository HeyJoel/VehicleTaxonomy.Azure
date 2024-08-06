using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Domain.Models;

namespace VehicleTaxonomy.Azure.Domain.Tests.Models.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddModelCommandHandlerTests
{
    const string UNIQUE_PREFIX = "AddModelCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public AddModelCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValid_CanAdd()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenValid_CanAdd);
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            if (!result.IsValid)
            {
                return;
            }

            result.Result.Id.Should().Be(id);
            dbRecord.Should().NotBeNull();

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
    }

    [Fact]
    public async Task WhenMakeNotExists_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenMakeNotExists_ReturnsError);
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddModelCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            Name = id
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddModelCommand.MakeId));
            error.Message.Should().MatchEquivalentOf("*make*exist*");
            result.Result.Should().BeNull();
        }
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddModelCommand.MakeId));
            result.Result.Should().BeNull();
        }
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddModelCommand.Name));
            result.Result.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsError);
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

        using (new AssertionScope())
        {
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ValidationErrors.Should().HaveCount(1);

            var error = result2.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddModelCommand.Name));
            error.Message.Should().Match("*already exists*");
        }
    }
}
