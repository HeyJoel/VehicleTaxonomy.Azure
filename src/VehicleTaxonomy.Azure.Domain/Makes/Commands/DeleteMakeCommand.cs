namespace VehicleTaxonomy.Azure.Domain.Makes;

/// <summary>
/// Delete an existing make. If the make does not exist then a validation
/// error is returned.
/// </summary>
public class DeleteMakeCommand
{
    /// <summary>
    /// Id of the make to delete e.g. "volkswagen", "mg" or "mercedes-benz".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;
}
