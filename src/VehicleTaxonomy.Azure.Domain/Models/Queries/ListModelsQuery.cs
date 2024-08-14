namespace VehicleTaxonomy.Azure.Domain.Models;

/// <summary>
/// Lists all the models that belong to a specific make.
/// </summary>
public class ListModelsQuery
{
    /// <summary>
    /// Required. The unique id of the parent make to filter models by
    /// e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;
}
