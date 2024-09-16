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
    /// Use retry policies when handling transient faults. Defaults to
    /// <see langword="true"/> but is useful for disable when running
    /// local tests so tests don't hang when there is a connection error.
    /// For CI I've found that the CosmosDb emulator can still have intermittent
    /// connection issues so I don't disable it there.
    /// </summary>
    public bool UseRetries { get; set; } = true;

    /// <summary>
    /// The request execution strategy to use when executing batch requests
    /// against a CosmosDb container. Defaults to <see cref="CosmosDbBatchStrategy.ParallelRequests"/>
    /// for best compatibility with low RU configurations.
    /// </summary>
    public CosmosDbBatchStrategy BatchStrategy { get; set; } = CosmosDbBatchStrategy.ParallelRequests;

    /// <summary>
    /// The maximum number of parallel requests to make when executing batchs of
    /// requests using <see cref="CosmosDbBatchStrategy.ParallelRequests"/>. Defaults
    /// to 1, running request in series to prevent overloading containers with low
    /// RU allocations.
    /// </summary>
    [Range(1, 1000)]
    public int BatchStrategyMaxParallelRequests { get; set; } = 1;
}
