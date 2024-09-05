using Microsoft.DurableTask.Client;
using VehicleTaxonomy.Azure.Infrastructure.DataImport;

namespace VehicleTaxonomy.Azure.Api;

public class DataImportTaxonomyGetStatusApi
{
    [OpenApiOperation(
        nameof(DataImportTaxonomyGetStatus),
        DataImportApiConstants.CollectionTag,
        Description = "Query the status and result data for an individual taxonomy data import session.")]
    [OpenApiParameter(
        nameof(instanceId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Unique id of the data import session to query.",
        Example = typeof(InstanceIdQueryParameterExample))]
    [OpenApiStandardResponseWithData<TaxonomyImportOrchestratorResult>(
        Description = "Returns status and result data for an individual taxonomy data import session. If the session could not be found then null is returned.",
        Example = typeof(ResultExample))]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(DataImportTaxonomyGetStatus))]
    public async Task<IActionResult> DataImportTaxonomyGetStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = DataImportApiConstants.RoutePrefix + "/taxonomy/{instanceId}")] HttpRequest req,
        [DurableClient] DurableTaskClient durableTaskClient,
        string instanceId,
        CancellationToken cancellationToken
        )
    {
        var taskMetaData = await durableTaskClient.GetInstanceAsync(instanceId, true, cancellationToken);
        TaxonomyImportOrchestratorResult? result = null;

        if (taskMetaData == null)
        {
            return ApiResponseHelper.ToResult(result);
        }

        result = taskMetaData.ReadCustomStatusAs<TaxonomyImportOrchestratorResult>() ?? new();
        result.StartedDate = taskMetaData.CreatedAt;

        if (taskMetaData.IsCompleted)
        {
            result.FinishedDate = taskMetaData.LastUpdatedAt;
        }

        return ApiResponseHelper.ToResult(result);
    }

    public class InstanceIdQueryParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "01J55QJ7YJCBX8FJHB4YVPXN6S";
    }

    public class ResultExample : OpenApiStandardResponseExampleBase<TaxonomyImportOrchestratorResult>
    {
        public override TaxonomyImportOrchestratorResult Example => new()
        {
            NumInvalid = 4,
            NumSkipped = 2,
            NumSuccess = 218,
            SkippedReasons = new Dictionary<string, ISet<int>>()
            {
                { "Invalid body type", new HashSet<int>() { 2, 4 }}
            },
            Status = DataImportJobStatus.Processing,
            ValidationErrors = new Dictionary<string, ISet<int>>()
            {
                { "Make name must be 50 characters or less", new HashSet<int>() { 6, 7 }},
                { "Variant name should be less than 100 characters", new HashSet<int>() { 8, 9 }},
            },
            BatchProcessed = 0,
            BatchSize = 1000,
            BatchTotal = 3,
            StartedDate = DateTimeOffset.UtcNow
        };
    }
}
