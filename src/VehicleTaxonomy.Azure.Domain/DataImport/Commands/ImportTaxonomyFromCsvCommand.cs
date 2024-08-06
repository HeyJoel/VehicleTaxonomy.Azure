namespace VehicleTaxonomy.Azure.Domain.DataImport;

public class ImportTaxonomyFromCsvCommand
{
    /// <summary>
    /// Toggle between importing data and a validation-only mode.
    /// Defaults to <see cref="DataImportMode.Validate"/>.
    /// </summary>
    public DataImportMode ImportMode { get; set; } = DataImportMode.Validate;

    /// <summary>
    /// CSV file to process.
    /// </summary>
    public IFileSource File { get; set; } = null!;
}
