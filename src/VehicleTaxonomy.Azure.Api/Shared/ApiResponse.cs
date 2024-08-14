namespace VehicleTaxonomy.Azure.Api;

/// <summary>
/// A simple wrapper for all API responses for consistency.
/// </summary>
public class ApiResponse<TResult>
{
    public bool IsValid { get; set; }

    public TResult? Result { get; set; }

    public IReadOnlyCollection<ValidationError> ValidationErrors { get; set; } = [];
}
