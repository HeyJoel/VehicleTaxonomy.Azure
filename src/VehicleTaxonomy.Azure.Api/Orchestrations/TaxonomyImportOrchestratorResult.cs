using VehicleTaxonomy.Azure.Infrastructure.DataImport;

namespace VehicleTaxonomy.Azure.Api;

public class TaxonomyImportOrchestratorResult : DataImportJobResult
{
    public DateTimeOffset StartedDate { get; set; }

    public DateTimeOffset? FinishedDate { get; set; }

    /// <summary>
    /// The maximum number of CSV rows per batch.
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// The total number of batches.  <see langword="null"/> means that
    /// batch splitting hasn't yet occured.
    /// </summary>
    public int? BatchTotal { get; set; }

    /// <summary>
    /// The number of batches that have been processed.
    /// </summary>
    public int BatchProcessed { get; set; }
}
