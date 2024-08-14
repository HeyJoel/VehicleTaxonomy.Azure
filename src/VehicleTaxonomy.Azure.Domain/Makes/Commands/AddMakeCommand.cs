namespace VehicleTaxonomy.Azure.Domain.Makes;

/// <summary>
/// Create a new make. If a make with the same name already exists then a
/// validation error is returned.
/// </summary>
public class AddMakeCommand
{
    /// <summary>
    /// The name of the make e.g. "Volkswagen", "MG" or "Mercedes Benz".
    /// This will be converted to an id that is passed back in the handler
    /// result.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
