namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// Used to simplify and consistently apply the same formatting
/// across APIs.
/// </summary>
public static class ApiResponseHelper
{
    public static IActionResult ToResult<TResult>(QueryResponse<TResult> queryResponse)
    {
        ArgumentNullException.ThrowIfNull(queryResponse);

        var apiResponse = new ApiResponse<TResult>()
        {
            IsValid = queryResponse.IsValid,
            Result = queryResponse.Result,
            ValidationErrors = queryResponse.ValidationErrors,
        };

        return ToActionResult(apiResponse);
    }

    public static IActionResult ToResult<TResult>(CommandResponse<TResult> commandResponse)
    {
        ArgumentNullException.ThrowIfNull(commandResponse);

        var apiResponse = new ApiResponse<TResult>()
        {
            IsValid = commandResponse.IsValid,
            Result = commandResponse.Result,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToActionResult(apiResponse);
    }

    public static IActionResult ToResult<TResult>(TResult result)
    {
        var apiResponse = new ApiResponse<TResult>()
        {
            IsValid = true,
            Result = result
        };

        return ToActionResult(apiResponse);
    }

    public static IActionResult ToResult(CommandResponse commandResponse)
    {
        ArgumentNullException.ThrowIfNull(commandResponse);

        var apiResponse = new ApiResponse<object>()
        {
            IsValid = commandResponse.IsValid,
            ValidationErrors = commandResponse.ValidationErrors,
        };

        return ToActionResult(apiResponse);
    }

    public static IActionResult ValidationError(ValidationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var apiResponse = new ApiResponse<object>()
        {
            IsValid = false,
            ValidationErrors = [error],
        };

        return ToActionResult(apiResponse);
    }

    public static IActionResult Success()
    {
        return ToActionResult(new ApiResponse<object>()
        {
            IsValid = true
        });
    }

    private static IActionResult ToActionResult<TResult>(ApiResponse<TResult> apiResponse)
    {
        if (apiResponse.IsValid)
        {
            return new OkObjectResult(apiResponse);
        }
        else
        {
            return new BadRequestObjectResult(apiResponse);
        }
    }
}
