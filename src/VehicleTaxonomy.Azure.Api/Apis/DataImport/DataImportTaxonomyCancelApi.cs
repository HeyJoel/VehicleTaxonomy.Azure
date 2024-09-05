using Microsoft.DurableTask.Client;

namespace VehicleTaxonomy.Azure.Api;

public class DataImportTaxonomyCancelApi
{
    [OpenApiOperation(
        nameof(DataImportTaxonomyCancel),
        DataImportApiConstants.CollectionTag,
        Description = "Terminate an in-progress data import session. If the session has already completed then a validation error is returned.")]
    [OpenApiParameter(
        nameof(instanceId),
        In = ParameterLocation.Path,
        Required = true,
        Description = "Unique id of the data import session to terminate.",
        Example = typeof(InstanceIdQueryParameterExample))]
    [OpenApiStandardSuccessResponse]
    [OpenApiStandardValidationErrorResponse]
    [Function(nameof(DataImportTaxonomyCancel))]
    public async Task<IActionResult> DataImportTaxonomyCancel(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = DataImportApiConstants.RoutePrefix + "/taxonomy/{instanceId}/cancel")] HttpRequest req,
        [DurableClient] DurableTaskClient durableTaskClient,
        string instanceId,
        CancellationToken cancellationToken
        )
    {
        var taskMetaData = await durableTaskClient.GetInstanceAsync(instanceId, cancellationToken);

        if (taskMetaData == null)
        {
            return ApiResponseHelper.ValidationError(new ValidationError(
                "Import job not found"
                ));
        }

        if (taskMetaData.IsCompleted)
        {
            return ApiResponseHelper.ValidationError(new ValidationError(
                "Cannot cancel task because it is already completed"
                ));
        }

        await durableTaskClient.TerminateInstanceAsync(instanceId, cancellationToken);

        return ApiResponseHelper.Success();
    }

    public class InstanceIdQueryParameterExample : OpenApiStandardExampleBase<string>
    {
        public override string Example => "01J55QJ7YJCBX8FJHB4YVPXN6S";
    }
}
