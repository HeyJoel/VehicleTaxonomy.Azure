using VehicleTaxonomy.Azure.Domain.DataImport;
using VehicleTaxonomy.Azure.Infrastructure.DataImport;

namespace VehicleTaxonomy.Azure.Api;

public class DataImportTaxonomyValidateApi
{
    private readonly ILogger<DataImportTaxonomyValidateApi> _logger;
    private readonly ImportTaxonomyFromCsvCommandHandler _importTaxonomyFromCsvCommandHandler;

    public DataImportTaxonomyValidateApi(
        ILogger<DataImportTaxonomyValidateApi> logger,
        ImportTaxonomyFromCsvCommandHandler importTaxonomyFromCsvCommandHandler
        )
    {
        _logger = logger;
        _importTaxonomyFromCsvCommandHandler = importTaxonomyFromCsvCommandHandler;
    }

    [OpenApiOperation(
        nameof(DataImportTaxonomyValidate),
        DataImportApiConstants.CollectionTag,
        Description = "Validate the contents of a taxonomy data import CSV file. Does not import any data.")]
    [OpenApiRequestBody(
        "multipart/form-data",
        typeof(Request),
        Required = true,
        Description = "CSV file to validate.")]
    [OpenApiStandardResponseWithData<DataImportJobResult>(
        Description = "Returns information for the validation step of the import job. The 'numSuccess' property will always be 0 because data is not imported.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(DataImportTaxonomyValidate))]
    public async Task<IActionResult> DataImportTaxonomyValidate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = DataImportApiConstants.RoutePrefix + "/taxonomy/validate")] HttpRequest req,
        CancellationToken cancellationToken
        )
    {
        var command = await DataImportTaxonomyApi.BindCommandAsync(_logger, req, nameof(DataImportTaxonomyValidate), DataImportMode.Validate, cancellationToken);
        var commandResponse = await _importTaxonomyFromCsvCommandHandler.ExecuteAsync(command, cancellationToken);

        return ApiResponseHelper.ToResult(commandResponse);
    }

    /// <summary>
    /// For OpenApi docs only.
    /// </summary>
    public class Request
    {
        public byte[] File { get; set; } = null!;
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<DataImportJobResult>
    {
        public override DataImportJobResult Example => new()
        {
            NumInvalid = 4,
            NumSkipped = 2,
            NumSuccess = 0,
            SkippedReasons = new Dictionary<string, ISet<int>>()
            {
                { "Invalid body type", new HashSet<int>() { 2, 4 }}
            },
            Status = DataImportJobStatus.Finished,
            ValidationErrors = new Dictionary<string, ISet<int>>()
            {
                { "Make name must be 50 characters or less", new HashSet<int>() { 6, 7 }},
                { "Variant name should be less than 100 characters", new HashSet<int>() { 8, 9 }},
            },
        };
    }
}
