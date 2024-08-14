namespace VehicleTaxonomy.Azure.Domain.Models;

/// <summary>
/// Delete an existing model. If the model does not exist then a validation
/// error is returned.
/// </summary>
public class DeleteModelCommand
{
    /// <summary>
    /// The unique id of the parent make that the model belongs
    /// to e.g. "volkswagen" or "bmw".
    /// </summary>
    public string MakeId { get; set; } = string.Empty;

    /// <summary>
    /// Id of the make to delete e.g. "polo", "305" or "3-series".
    /// </summary>
    public string ModelId { get; set; } = string.Empty;
}
