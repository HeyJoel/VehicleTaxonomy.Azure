using VehicleTaxonomy.Azure.Domain.DataImport;

namespace VehicleTaxonomy.Azure.Api.Apis;

public class DataImportApi
{
    const string ROUTE_PREFIX = "data-import";

    private readonly ILogger<DataImportApi> _logger;
    private readonly ImportTaxonomyFromCsvCommandHandler _importTaxonomyFromCsvCommandHandler;

    public DataImportApi(
        ILogger<DataImportApi> logger,
        ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler
        )
    {
        _logger = logger;
        _importTaxonomyFromCsvCommandHandler = importTaxonomyFromCsvCommandHandler;
    }

    [Function(nameof(DataImportTaxonomy))]
    public async Task<IActionResult> DataImportTaxonomy(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ROUTE_PREFIX + "/taxonomy")] HttpRequest req,
        CancellationToken cancellationToken
        )
    {
        return await ExecuteImport(req, nameof(DataImportTaxonomy), DataImportMode.Run, cancellationToken);
    }

    [Function(nameof(DataImportTaxonomyValidate))]
    public async Task<IActionResult> DataImportTaxonomyValidate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = ROUTE_PREFIX + "/taxonomy/validate")] HttpRequest req,
        CancellationToken cancellationToken
        )
    {
        return await ExecuteImport(req, nameof(DataImportTaxonomyValidate), DataImportMode.Validate, cancellationToken);
    }

    private async Task<IActionResult> ExecuteImport(
        HttpRequest req,
        string functionName,
        DataImportMode dataImportMode,
        CancellationToken cancellationToken
        )
    {
        var formData = await req.ReadFormAsync(cancellationToken);
        var file = formData.Files[nameof(ImportTaxonomyFromCsvCommand.File)];

        _logger.LogInformation("Triggered {Function} with file '{FileName}', size {FileSizeInBytes}b", functionName, file?.FileName, file?.Length);

        var command = new ImportTaxonomyFromCsvCommand()
        {
            ImportMode = dataImportMode
        };

        if (file != null)
        {
            command.File = new FormFileSource(file);
        }

        var commandResponse = await _importTaxonomyFromCsvCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }
}
