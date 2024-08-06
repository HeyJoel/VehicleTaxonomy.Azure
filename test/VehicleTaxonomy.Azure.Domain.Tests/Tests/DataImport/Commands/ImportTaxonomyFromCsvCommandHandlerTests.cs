using Meziantou.Framework.InlineSnapshotTesting;
using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Domain.DataImport;
using VehicleTaxonomy.Azure.Domain.Tests.Makes;
using VehicleTaxonomy.Azure.Domain.Tests.Models;
using VehicleTaxonomy.Azure.Domain.Tests.Variants;

namespace VehicleTaxonomy.Azure.Domain.Tests.DataImport;

[Collection(nameof(DbDependentFixtureCollection))]
public class ImportTaxonomyFromCsvCommandHandlerTests
{
    private readonly DbDependentFixture _dbDependentFixture;

    public ImportTaxonomyFromCsvCommandHandlerTests(
        DbDependentFixture dbDependentFixture
        )
    {
        _dbDependentFixture = dbDependentFixture;
    }

    [Fact]
    public async Task WhenValidSingleRow_CanImport()
    {
        var makeId = "imptaxcsvch-valsrow";
        var modelId = "abarth-124";
        var variantId = "124-gt-multiair-1-4l-petrol";

        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();
        var modelTestHelper = scope.ServiceProvider.GetRequiredService<ModelTestHelper>();
        var variantTestHelper = scope.ServiceProvider.GetRequiredService<VariantTestHelper>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenValidSingleRow_CanImport)),
            ImportMode = DataImportMode.Run
        });

        var dbMake = await makeTestHelper.GetRawDocumentAsync(makeId);
        var dbModel = await modelTestHelper.GetRawDocumentAsync(makeId, modelId);
        var dbVariant = await variantTestHelper.GetRawDocumentAsync(makeId, modelId, variantId);

        response.IsValid.Should().BeTrue();

        InlineSnapshot.Validate(response.Result, """
            NumSuccess: 1
            Status: Finished
            SkippedReasons: {}
            ValidationErrors: {}
            """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbMake, """
                ParentPath: /
                PublicId: imptaxcsvch-valsrow
                Name: ImpTaxCsvCH ValSRow
                CreateDate: 2024-07-16T08:23:56
                """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbModel, """
                EntityType: Model
                ParentPath: /imptaxcsvch-valsrow
                PublicId: abarth-124
                Name: ABARTH 124
                CreateDate: 2024-07-16T08:23:56
                """);

        InlineSnapshot
            .WithSettings(InlineSnapshotSettingsLibrary.IgnoreDefaultOrEmptyCollection)
            .Validate(dbVariant, """
                EntityType: Variant
                ParentPath: /imptaxcsvch-valsrow/abarth-124
                PublicId: 124-gt-multiair-1-4l-petrol
                Name: 124 GT MULTIAIR 1.4l Petrol
                CreateDate: 2024-07-16T08:23:56
                VariantData:
                  FuelCategory: Petrol
                  EngineSizeInCC: 1400
                """);
    }

    [Fact]
    public async Task WhenValidMultiRow_CanImport()
    {
        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenValidMultiRow_CanImport)),
            ImportMode = DataImportMode.Run
        });

        response.IsValid.Should().BeTrue();
        InlineSnapshot.Validate(response.Result, """
            NumSuccess: 51
            NumSkipped: 5
            NumInvalid: 2
            Status: Finished
            SkippedReasons:
              Model is empty:
                - 4
              Invalid body type:
                - 5
                - 6
                - 9
                - 10
            ValidationErrors:
              The length of 'Model Name' must be 50 characters or fewer:
                - 53
              The length of 'Make Name' must be 50 characters or fewer:
                - 54
            """);
    }

    [Fact]
    public async Task WhenModeValidate_DoesNotImport()
    {
        var makeId = "imptaxcsvch-modeval-notimp";

        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();
        var makeTestHelper = scope.ServiceProvider.GetRequiredService<MakeTestHelper>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync(nameof(WhenModeValidate_DoesNotImport)),
            ImportMode = DataImportMode.Validate
        });

        var dbMake = await makeTestHelper.GetRawDocumentAsync(makeId);

        using (new AssertionScope())
        {
            dbMake.Should().BeNull();
            response.IsValid.Should().BeTrue();
            InlineSnapshot.Validate(response.Result, """
                NumSuccess: 1
                Status: Finished
                SkippedReasons: {}
                ValidationErrors: {}
                """);
        }
    }

    [Fact]
    public async Task WhenInvalidFile_ReturnsError()
    {
        var scope = _dbDependentFixture.ServiceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ImportTaxonomyFromCsvCommandHandler>();

        var response = await handler.ExecuteAsync(new()
        {
            File = GetTestFileSourceAsync("BadFile"),
            ImportMode = DataImportMode.Run
        });

        using (new AssertionScope())
        {
            response.IsValid.Should().BeFalse();
            response.ValidationErrors.Should().HaveCount(1);
            var error = response.ValidationErrors.First();
            error.Property.Should().Be(nameof(ImportTaxonomyFromCsvCommand.File));
            response.Result.Should().BeNull();
        }
    }

    private EmbeddedResourceFileSource GetTestFileSourceAsync(string uniqueName)
    {
        var fileSource = new EmbeddedResourceFileSource(
            GetType().Assembly,
            "VehicleTaxonomy.Azure.Domain.Tests.Tests.DataImport.TestResources",
            $"{nameof(ImportTaxonomyFromCsvCommandHandlerTests)}_{uniqueName}.csv"
            );

        return fileSource;
    }
}
