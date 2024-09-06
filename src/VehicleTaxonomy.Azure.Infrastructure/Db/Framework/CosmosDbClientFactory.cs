using Microsoft.Azure.Cosmos;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// Used to handle singleton instances of CosmosDb clients for multiple
/// configurations. Specifically this is to get around the fact that you
/// can't switch between bulk-mode with a single client. This is a problem
/// because bulk-mode reduces performance of non-bulk operations, typically
/// adding a second to each operation while it waits for more operations to
/// batch. CosmosDb clients should be singletons to avoid port exaughstion
/// with the underlaying HttpClient. <see cref="CosmosDbClientFactory"/>
/// should therefore be registered as singleton. See <see href="https://learn.microsoft.com/en-gb/azure/cosmos-db/nosql/performance-tips-dotnet-sdk-v3?tabs=trace-net-core#sdk-usage"/>
/// for more info.
/// </summary>
public sealed class CosmosDbClientFactory : IDisposable
{
    private readonly Lazy<CosmosClient> _defaultClient;
    private readonly Lazy<CosmosClient> _bulkClient;
    private readonly CosmosDbOptions _cosmosDbOptions;

    public CosmosDbClientFactory(
        CosmosDbOptions cosmosDbOptions
        )
    {
        _cosmosDbOptions = cosmosDbOptions;
        _defaultClient = new(() => CreateClient(false));
        _bulkClient = new(() => CreateClient(true));
    }

    public CosmosClient Get(bool allowBulkExecution = false)
    {
        if (allowBulkExecution)
        {
            return _bulkClient.Value;
        }

        return _defaultClient.Value;
    }

    private CosmosClient CreateClient(bool allowBulkExecution)
    {
        var clientOptions = new CosmosClientOptions()
        {
            AllowBulkExecution = allowBulkExecution,
            SerializerOptions = new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
        };

        if (_cosmosDbOptions.UseLocalDb)
        {
            // Ignore local self-signed cert
            clientOptions.ServerCertificateCustomValidationCallback = (c, h, e) => true;

            // local only works in Gateway mode
            clientOptions.ConnectionMode = ConnectionMode.Gateway;
            clientOptions.LimitToEndpoint = true;

            // Don't hang around if we get a connection issue
            clientOptions.MaxRetryAttemptsOnRateLimitedRequests = 0;
        }

        return new CosmosClient(_cosmosDbOptions.ConnectionString, clientOptions);
    }

    public void Dispose()
    {
        DisposeClient(_defaultClient);
        DisposeClient(_bulkClient);
    }

    private static void DisposeClient(Lazy<CosmosClient> client)
    {
        if (client.IsValueCreated)
        {
            client.Value?.Dispose();
        }
    }
}
