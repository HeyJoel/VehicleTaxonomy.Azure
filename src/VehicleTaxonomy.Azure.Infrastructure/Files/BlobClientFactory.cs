using System.Collections.Concurrent;
using Azure.Storage.Blobs;

namespace VehicleTaxonomy.Azure.Infrastructure.Files;

/// <summary>
/// Factory that handles caching various blob clients for the duration of
/// the factory lifetime, which should typically be registered as singleton.
/// See <see cref="https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-client-management?tabs=dotnet#manage-client-objects"/>
/// for client lifetime guidance.
/// </summary>
public class BlobClientFactory
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ConcurrentDictionary<string, BlobContainerClient> _containerClients = new(StringComparer.OrdinalIgnoreCase);

    public BlobClientFactory(
        BlobStorageOptions blobStorageOptions
        )
    {
        if (string.IsNullOrWhiteSpace(blobStorageOptions.ConnectionString))
        {
            throw new InvalidOperationException($"The {typeof(BlobStorageOptions)}.{nameof(BlobStorageOptions.ConnectionString)} option is required to use the {nameof(BlobClientFactory)}");
        }
        _blobServiceClient = new BlobServiceClient(blobStorageOptions.ConnectionString);
    }

    public BlobServiceClient GetServiceClient()
    {
        return _blobServiceClient;
    }

    public BlobContainerClient GetContainerClient(string containerName)
    {
        return _containerClients.GetOrAdd(containerName, _blobServiceClient.GetBlobContainerClient);
    }
}
