using Microsoft.Azure.Cosmos;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// Defines a basic table schema for use with <see cref="CosmosDbContainerInitializer"/>.
/// </summary>
public interface ICosmosDbContainerDefinition
{
    /// <summary>
    /// The container name forms a segment of the URI used to access the container so although
    /// there's no documented character restrictions, I'm sure there are so best stick to
    /// url-safe characters e.g. "chat-message". Max length 255.
    /// </summary>
    string ContainerName { get; }

    /// <summary>
    /// Returns the property and indexing definition used to create the container.
    /// </summary>
    ContainerProperties GetContainerProperties();
}
