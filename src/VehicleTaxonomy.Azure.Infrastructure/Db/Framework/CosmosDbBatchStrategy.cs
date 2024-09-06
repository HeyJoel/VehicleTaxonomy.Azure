namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// The request execution strategy to use when executing batch requests
/// against a CosmosDb container.
/// </summary>
public enum CosmosDbBatchStrategy
{
    /// <summary>
    /// Send batches of requests in parallel up to a configurable degree
    /// of parallelism.
    /// </summary>
    ParallelRequests,

    /// <summary>
    /// Utilize <see cref="https://devblogs.microsoft.com/cosmosdb/introducing-bulk-support-in-the-net-sdk/">bulk execution</see>
    /// functionality to acheive mass-throughput. Using this option requires
    /// high provisioned throughput which won't be suitable for most situations
    /// but has been included to evaluate and experiment with the feature. If
    /// RU capacity is exceeded then an exception will be thrown. When using the
    /// local emulator I had timeout issues with anything other than small batches.
    /// </summary>
    BulkExecution
}
