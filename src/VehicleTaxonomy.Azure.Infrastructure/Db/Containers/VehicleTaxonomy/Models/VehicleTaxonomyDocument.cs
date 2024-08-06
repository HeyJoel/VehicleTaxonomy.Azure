namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// The CosmosDb record that represents all vehicle taxonomy items, overloaded
/// to support multiple entity types i.e. Make, Model and Variant.
/// </summary>
public class VehicleTaxonomyDocument
{
    /// <summary>
    /// Used to determine the entity type in the overload cosmos db container.
    /// </summary>
    public VehicleTaxonomyEntity EntityType { get; set; }

    /// <summary>
    /// The internal CosmosDb identifier for the entity, unique across all entity types. This is formed
    /// from the complete partition key, which is then hashed and encoded - CosmosDb recommendation
    /// is alpha-numerical only for interoperability with ADF, Spark, Kafka etc.
    /// </summary>
    /// <remarks>
    /// Internal because this is calculated when a new entity is created.
    /// </remarks>
    public string Id { get; internal set; } = string.Empty;

    /// <summary>
    /// The hierarchical path to the entity using the slugs of parent entities. e.g. for a model
    /// this might be '/volkswagen', or for a variant this might be '/volkswagen/polo'. For
    /// Makes this will be "/". This value is used as the partition key.
    /// </summary>
    public string ParentPath { get; set; } = string.Empty;

    /// <summary>
    /// A slugified version of the name used as the public identifier for item, unique within
    /// the <see cref="ParentPath"/> e.g. "volkswagen", "3-series" or "se-petrol-1-3".
    /// </summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>
    /// Publically displayable name or title of the record e.g. "BMW" or
    /// "3 Series".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The date the record was created, in UTC.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Additional fields relating only to "variant" entities.
    /// </summary>
    public VariantData? VariantData { get; set; }
}
