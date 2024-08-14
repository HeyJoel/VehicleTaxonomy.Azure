using Microsoft.DurableTask;
using VehicleTaxonomy.Azure.Domain.DataImport;
using VehicleTaxonomy.Azure.Infrastructure;
using VehicleTaxonomy.Azure.Infrastructure.DataImport;
using VehicleTaxonomy.Azure.Infrastructure.Files;

namespace VehicleTaxonomy.Azure.Api.Orchestrations;

/// <summary>
/// Orchestrator to handle taxonomy import with large CSV file uploads, splitting
/// the CSV file into batches and running them in series to reduce the risk of
/// function timeouts.
/// </summary>
public class TaxonomyImportOrchestrator
{
    public const string EntryPoint = nameof(TaxonomyImportOrchestrator) + nameof(Orchestrator);
    private const int MaxRowsPerFile = 10000;

    private readonly BlobClientFactory _blobClientFactory;
    private readonly ImportTaxonomyFromCsvCommandHandler _importTaxonomyFromCsvCommandHandler;

    public TaxonomyImportOrchestrator(
        BlobClientFactory blobClientFactory,
        ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler
        )
    {
        _blobClientFactory = blobClientFactory;
        _importTaxonomyFromCsvCommandHandler = importTaxonomyFromCsvCommandHandler;
    }

    /// <summary>
    /// Orchestrator entry point, called from the data import API endpoint.
    /// </summary>
    [Function(EntryPoint)]
    public static async Task Orchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        string fileName,
        CancellationToken cancellationToken = default
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var result = new TaxonomyImportOrchestratorResult()
        {
            Status = DataImportJobStatus.Processing,
            BatchSize = MaxRowsPerFile
        };
        context.SetCustomStatus(result);

        var fileBatches = await context.CallActivityAsync<List<string>>(
            nameof(TaxonomyImportOrchestrator) + nameof(SplitCsv),
            fileName
            );

        result.BatchTotal = fileBatches.Count;
        context.SetCustomStatus(result);

        // Process in series to avoid overloading CosmosDB RUs
        foreach (var fileBatch in fileBatches)
        {
            var batchResult = await context.CallActivityAsync<CommandResponse<DataImportJobResult>>(
                nameof(TaxonomyImportOrchestrator) + nameof(ImportBatch),
                fileBatch
                );

            // CSV is pre-validated so this should not occur
            batchResult.ThrowIfInvalid();
            ShouldNotBeNullException.ThrowIfNull(batchResult.Result);

            AppendBatchResult(result, batchResult.Result);
            result.BatchProcessed++;

            context.SetCustomStatus(result);

            if (result.Status == DataImportJobStatus.FatalError)
            {
                break;
            }
        }

        if (result.Status != DataImportJobStatus.FatalError)
        {
            result.Status = DataImportJobStatus.Finished;
            context.SetCustomStatus(result);
        }
    }

    /// <summary>
    /// Activity to split the CSV file into batches and upload each
    /// part of the blob container.
    /// </summary>
    [Function(nameof(TaxonomyImportOrchestrator) + nameof(SplitCsv))]
    public async Task<List<string>> SplitCsv(
        [ActivityTrigger] string fileName,
        CancellationToken cancellationToken = default
        )
    {
        var container = _blobClientFactory.GetContainerClient(TaxonomyImportBlobContainer.ContainerName);
        var blobClient = container.GetBlobClient(fileName);

        using var file = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

        List<string> fileNames = [];

        await CsvFileSplitter.SplitAsync(
            file,
            MaxRowsPerFile,
            FileWriter,
            cancellationToken
            );

        async Task FileWriter(CsvFileSplitterResult result)
        {
            var prefix = Path.GetFileNameWithoutExtension(fileName);
            var batchFileName = $"{prefix}-batch-{result.BatchNumber}.csv";

            var blobClient = container.GetBlobClient(batchFileName);
            await blobClient.UploadAsync(result.File, cancellationToken);

            fileNames.Add(batchFileName);
        }

        return fileNames;
    }

    /// <summary>
    /// Activity to process a single batch (CSV file).
    /// </summary>
    [Function(nameof(TaxonomyImportOrchestrator) + nameof(ImportBatch))]
    public async Task<CommandResponse<DataImportJobResult>> ImportBatch(
        [ActivityTrigger] string fileName,
        CancellationToken cancellationToken = default
        )
    {
        var container = _blobClientFactory.GetContainerClient(TaxonomyImportBlobContainer.ContainerName);
        var blobClient = container.GetBlobClient(fileName);

        using var file = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

        var commandResponse = await _importTaxonomyFromCsvCommandHandler.ExecuteAsync(new()
        {
            ImportMode = DataImportMode.Run,
            File = new StreamFileSource(fileName, () => file)
        }, cancellationToken);

        return commandResponse;
    }

    private static void AppendBatchResult(DataImportJobResult result, DataImportJobResult resultToAppend)
    {
        result.NumInvalid += resultToAppend.NumInvalid;
        result.NumSkipped += resultToAppend.NumSkipped;
        result.NumSuccess += resultToAppend.NumSuccess;

        MergeSets(result.SkippedReasons, resultToAppend.SkippedReasons);
        MergeSets(result.ValidationErrors, resultToAppend.ValidationErrors);

        if (resultToAppend.Status == DataImportJobStatus.FatalError)
        {
            result.Status = resultToAppend.Status;
        }
    }

    private static void MergeSets(
        IDictionary<string, ISet<int>> messages,
        IDictionary<string, ISet<int>> messagesToMerge
        )
    {
        const int maxMessageValues = 500;

        var numMessageValues = messages
            .Select(m => m.Value.Count)
            .Aggregate(0, (result, current) => result + current);

        foreach (var messageToMerge in messagesToMerge)
        {
            messages.TryGetValue(messageToMerge.Key, out var existingMessage);

            foreach (var valueToMerge in messageToMerge.Value)
            {
                // a max message cap prevents an overly large result.
                if (numMessageValues >= maxMessageValues)
                {
                    return;
                }

                if (existingMessage == null)
                {
                    existingMessage = new HashSet<int>();
                    messages.Add(messageToMerge.Key, existingMessage);
                }

                existingMessage.Add(valueToMerge);
            }
        }
    }
}
