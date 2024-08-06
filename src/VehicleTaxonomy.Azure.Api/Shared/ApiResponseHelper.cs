namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Used to simplify and consistently apply the same formatting
/// across APIs.
/// </summary>
public static class ApiResponseHelper
{
    public static IActionResult ToResult<TResult>(QueryResponse<TResult> queryResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = queryResponse.IsValid,
            Result = queryResponse.Result,
            ValidationErrors = queryResponse.ValidationErrors,
        };

        return ToResult(queryResponse, apiResponse);
    }

    public static IActionResult ToResult<TResult>(CommandResponse<TResult> commandResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = commandResponse.IsValid,
            Result = commandResponse.Result,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToResult(commandResponse, apiResponse);
    }

    public static IActionResult ToResult(CommandResponse commandResponse)
    {
        var apiResponse = new ApiResponse()
        {
            IsValid = commandResponse.IsValid,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToResult(commandResponse, apiResponse);
    }

    private static IActionResult ToResult(ICommandOrQueryResponse commandOrQueryResponse, ApiResponse apiResponse)
    {
        if (commandOrQueryResponse.IsValid)
        {
            return new OkObjectResult(apiResponse);
        }
        else
        {
            return new BadRequestObjectResult(apiResponse);
        }
    }
}
