using Microsoft.DurableTask.Client;
using VehicleTaxonomy.Azure.Api.Orchestrations;
using VehicleTaxonomy.Azure.Domain.DataImport;
using VehicleTaxonomy.Azure.Infrastructure.Files;

namespace VehicleTaxonomy.Azure.Api;

public class DataImportTaxonomyApi
{
    private readonly ILogger<DataImportTaxonomyApi> _logger;
    private readonly BlobClientFactory _blobClientFactory;
    private readonly ImportTaxonomyFromCsvCommandHandler _importTaxonomyFromCsvCommandHandler;

    public DataImportTaxonomyApi(
        ILogger<DataImportTaxonomyApi> logger,
        BlobClientFactory blobClientFactory,
        ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler
        )
    {
        _logger = logger;
        _blobClientFactory = blobClientFactory;
        _importTaxonomyFromCsvCommandHandler = importTaxonomyFromCsvCommandHandler;
    }

    [OpenApiOperation(
        nameof(DataImportTaxonomy),
        DataImportApiConstants.CollectionTag,
        Description = "Initiate a new taxonomy data import job.")]
    [OpenApiRequestBody(
        "multipart/form-data",
        typeof(Request),
        Required = true,
        Description = "CSV file to process.")]
    [OpenApiStandardResponseWithData<Result>(
        Description = "Returns a unique identifier for the import session that can be used to query and manage the session.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(DataImportTaxonomy))]
    public async Task<IActionResult> DataImportTaxonomy(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = DataImportApiConstants.RoutePrefix + "/taxonomy")] HttpRequest req,
        [DurableClient] DurableTaskClient durableTaskClient,
        CancellationToken cancellationToken
        )
    {
        var command = await BindCommandAsync(_logger, req, nameof(DataImportTaxonomy), DataImportMode.Validate, cancellationToken);
        if (command.File == null)
        {
            return ApiResponseHelper.ValidationError(new()
            {
                Message = "File is required",
                Property = nameof(command.File)
            });
        }

        // A sortable unique identifier makes it easier to look up files when debugging.
        // Note that leaking the timestamp via the Ulid isn't a concern for us here.
        var instanceId = Ulid.NewUlid().ToString();
        var fileName = $"{instanceId}.csv";

        var containerClient = _blobClientFactory.GetContainerClient(TaxonomyImportBlobContainer.ContainerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        using var csvStream = await command.File.OpenReadStreamAsync();
        await blobClient.UploadAsync(csvStream, cancellationToken);

        // Use an orchestrator to process the CSV file in chunks
        await durableTaskClient.ScheduleNewOrchestrationInstanceAsync(
            TaxonomyImportOrchestrator.EntryPoint,
            fileName,
            new()
            {
                InstanceId = instanceId
            },
            cancellationToken);

        return ApiResponseHelper.ToResult(new Result()
        {
            InstanceId = instanceId
        });
    }

    /// <summary>
    /// Model binding of IFormFile not supported, instead bind the file manually.
    /// </summary>
    internal static async Task<ImportTaxonomyFromCsvCommand> BindCommandAsync(
        ILogger logger,
        HttpRequest req,
        string functionName,
        DataImportMode dataImportMode,
        CancellationToken cancellationToken
        )
    {
        var formData = await req.ReadFormAsync(cancellationToken);
        var file = formData.Files[nameof(ImportTaxonomyFromCsvCommand.File)];

        logger.LogInformation("Triggered {Function} with file '{FileName}', size {FileSizeInBytes}b", functionName, file?.FileName, file?.Length);

        var command = new ImportTaxonomyFromCsvCommand()
        {
            ImportMode = dataImportMode
        };

        if (file != null)
        {
            command.File = new FormFileSource(file);
        }

        return command;
    }

    /// <summary>
    /// For OpenApi docs only.
    /// </summary>
    public class Request
    {
        public byte[] File { get; set; } = null!;
    }

    public class Result
    {
        public string InstanceId { get; set; } = string.Empty;
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<Result>
    {
        public override Result Example => new()
        {
            InstanceId = "01J55QJ7YJCBX8FJHB4YVPXN6S"
        };
    }
}
