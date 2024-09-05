using Microsoft.Azure.Cosmos;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// Defines the schema of the VehicleTaxonomy container. This is currently
/// intended mainly for standing up local test databases and only considers
/// schema configuration. For real databases we use IaC.
/// </summary>
public class VehicleTaxonomyContainerDefinition : ICosmosDbContainerDefinition
{
    public static readonly VehicleTaxonomyContainerDefinition Instance = new();

    public const int MakeNameMaxLength = 50;
    public const int ModelNameMaxLength = 50;
    public const int VariantNameMaxLength = 100;

    public string ContainerName { get; } = "vehicle-taxonomy";

    public ContainerProperties GetContainerProperties()
    {
        var containerProperties = new ContainerProperties()
        {

            Id = ContainerName,
            PartitionKeyPath = "/parentPath"
        };

        // Use include-only indexing to save RUs on writes
        containerProperties.IndexingPolicy.ExcludedPaths.Add(new() { Path = "/*" });
        containerProperties.IndexingPolicy.IncludedPaths.Add(new() { Path = "/parentPath/?" });
        containerProperties.IndexingPolicy.IncludedPaths.Add(new() { Path = "/name/?" });

        return containerProperties;
    }
}
