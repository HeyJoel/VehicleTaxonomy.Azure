using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Domain.Variants;

namespace VehicleTaxonomy.Azure.Domain.Tests.Variants.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class AddVariantCommandHandlerTests
{
    const string UNIQUE_PREFIX = "AddVariantCH_";

    private readonly DbDependentFixture _dbDependentFixture;

    public AddVariantCommandHandlerTests(
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
                    EntityType: Variant
                    ParentPath: /addvariantch-whenvalid-canaddmk/addvariantch-whenvalid-canaddmd
                    PublicId: addvariantch-whenvalid-canadd
                    Name: AddVariantCH_WhenValid_CanAdd
                    CreateDate: 2024-07-16T08:23:56
                    VariantData: {}
                    """);
        }
    }

    [Fact]
    public async Task CanAddWithOptionalProperties()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(CanAddWithOptionalProperties);
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeTrue();
            if (!result.IsValid)
            {
                return;
            }

            dbRecord.Should().NotBeNull();

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
    }

    [Fact]
    public async Task WhenModelNotExists_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenModelNotExists_ReturnsError);
        var id = EntityIdFormatter.Format(uniqueData);

        using var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<AddVariantCommandHandler>();

        var result = await handler.ExecuteAsync(new()
        {
            MakeId = id,
            ModelId = id,
            Name = id
        });

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddVariantCommand.ModelId));
            error.Message.Should().MatchEquivalentOf("*model*exist*");
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddVariantCommand.ModelId));
            result.Result.Should().BeNull();
        }
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

        using (new AssertionScope())
        {
            result.IsValid.Should().BeFalse();
            result.ValidationErrors.Should().HaveCount(1);
            var error = result.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddVariantCommand.Name));
            result.Result.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenNameNotUnique_ReturnsError()
    {
        const string uniqueData = UNIQUE_PREFIX + nameof(WhenNameNotUnique_ReturnsError);
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

        using (new AssertionScope())
        {
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ValidationErrors.Should().HaveCount(1);

            var error = result2.ValidationErrors.First();
            error.Property.Should().Be(nameof(AddVariantCommand.Name));
            error.Message.Should().Match("*already exists*");
        }
    }
}
