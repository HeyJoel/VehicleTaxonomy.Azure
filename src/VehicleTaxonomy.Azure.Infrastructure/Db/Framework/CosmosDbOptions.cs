using System.ComponentModel.DataAnnotations;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

public class CosmosDbOptions
{
    /// <summary>
    /// Configuration section name in an appsettings.json
    /// file or similar.
    /// </summary>
    public const string SectionName = "CosmosDb";

    /// <summary>
    /// Used to connect and authenticate with the CosmosDb instance.
    /// For integration tests this can be left as <see cref="string.Empty"/>
    /// which indicates that the CosmosDb TestContainer should be used.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Optionally specify a custom name for the database. Defaults
    /// to "vehicle-taxonomy".
    /// </summary>
    public string DatabaseName { get; set; } = "vehicle-taxonomy";

    /// <summary>
    /// When <see langword="true"/>, indicates that <see cref="ConnectionString"/>
    /// points to a local CosmosDb emulator instance and certificate errors
    /// should be ignored. Defaults to <see langword="false"/>.
    /// </summary>
    public bool UseLocalDb { get; set; }

    /// <summary>
    /// The request execution strategy to use when executing batch requests
    /// against a CosmosDb container. Defaults to <see cref="CosmosDbBatchStrategy.ParallelRequests"/>
    /// for best compatibility with low RU configurations.
    /// </summary>
    public CosmosDbBatchStrategy BatchStrategy { get; set; } = CosmosDbBatchStrategy.ParallelRequests;

    /// <summary>
    /// The maximum number of parallel requests to make when executing batchs of
    /// requests using <see cref="CosmosDbBatchStrategy.ParallelRequests"/>.
    /// </summary>
    [Range(1, 1000)]
    public int BatchStrategyMaxParallelRequests { get; set; } = 4;
}
