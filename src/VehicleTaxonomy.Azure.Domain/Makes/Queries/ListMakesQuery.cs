namespace VehicleTaxonomy.Azure.Domain.Makes;

/// <summary>
/// Lists all makes, optionally filtered by name.
/// </summary>
public class ListMakesQuery
{
    /// <summary>
    /// Optionally filter by the make name, using a case-insensitive
    /// "starts-with" comparision.
    /// </summary>
    public string? Name { get; set; }
}
