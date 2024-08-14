using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Cosmos;
using Sqids;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

public class VehicleTaxonomyRepository : IVehicleTaxonomyRepository
{
    private static readonly SqidsEncoder<byte> _sqidsEncoder = new(new()
    {
        Alphabet = "7eknlt3fiwdgmscayzq56phbjrxuo2v4",
    });

    private readonly CosmosDbClientFactory _cosmosDbClientFactory;
    private readonly CosmosDbOptions _cosmosDbOptions;

    public VehicleTaxonomyRepository(
        CosmosDbClientFactory cosmosDbClientFactory,
        CosmosDbOptions cosmosDbOptions
        )
    {
        _cosmosDbClientFactory = cosmosDbClientFactory;
        _cosmosDbOptions = cosmosDbOptions;
    }

    public async Task AddAsync(VehicleTaxonomyDocument taxonomy, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();

        // Id generation is encapsulated here 
        taxonomy.Id = CreateItemId(taxonomy.ParentPath, taxonomy.PublicId);

        // Although a partition key reference can be inferred from the model,
        // building it manually can improve performance according to the docs.
        var partitionKey = new PartitionKey(taxonomy.ParentPath);

        await container.CreateItemAsync(taxonomy, partitionKey, requestOptions: new()
        {
            EnableContentResponseOnWrite = false
        }, cancellationToken: cancellationToken);
    }

    public async Task AddOrUpdateBatchAsync(IEnumerable<VehicleTaxonomyDocument> taxonomies, CancellationToken cancellationToken = default)
    {
        var container = GetContainer(true);

        var concurrentTasks = new List<Task>();
        foreach (var taxonomy in taxonomies)
        {
            taxonomy.Id = CreateItemId(taxonomy.ParentPath, taxonomy.PublicId);
            var partitionKey = new PartitionKey(taxonomy.ParentPath);
            var task = container.UpsertItemAsync(taxonomy, partitionKey, requestOptions: new()
            {
                EnableContentResponseOnWrite = false
            }, cancellationToken: cancellationToken);

            concurrentTasks.Add(task);
        }

        await Task.WhenAll(concurrentTasks);
    }

    public async Task DeleteByIdAsync(VehicleTaxonomyEntity entityType, string entityId, string? parentMakeId, string? parentModelId, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();
        var parentPath = CreateParentPath(parentMakeId, parentModelId);
        var partitionKey = new PartitionKey(parentPath);
        var id = CreateItemId(parentPath, entityId);

        await container.DeleteItemAsync<VehicleTaxonomyDocument>(id, partitionKey, requestOptions: new()
        {
            EnableContentResponseOnWrite = false
        }, cancellationToken: cancellationToken);

        // TODO: cascade deletes
        // Note: delete by partition key is in preview
        // https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-delete-by-partition-key?tabs=dotnet-example
    }

    public async Task<VehicleTaxonomyDocument?> GetByIdAsync(
        VehicleTaxonomyEntity entityType,
        string entityId,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var container = GetContainer();
        var parentPath = CreateParentPath(parentMakeId, parentModelId);
        var partitionKey = new PartitionKey(parentPath);
        var id = CreateItemId(parentPath, entityId);

        try
        {
            var response = await container.ReadItemAsync<VehicleTaxonomyDocument>(id, partitionKey, cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyCollection<VehicleTaxonomyDocument>> ListAsync(
        VehicleTaxonomyEntity entityType,
        string? parentMakeId,
        string? parentModelId,
        CancellationToken cancellationToken = default
        )
    {
        var container = GetContainer();
        var parentPath = CreateParentPath(parentMakeId, parentModelId);

        var queryText = $"""
            select *
            from t
            where t.parentPath = @parentPath
            order by t.name
            """;

        var query = new QueryDefinition(queryText)
            .WithParameter("@parentPath", parentPath);

        using var feed = container.GetItemQueryIterator<VehicleTaxonomyDocument>(query);

        var result = new List<VehicleTaxonomyDocument>();
        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync(cancellationToken);
            result.AddRange(response);
        }

        return result;
    }

    private Container GetContainer(bool allowBulkExecution = false)
    {
        var cosmosClient = _cosmosDbClientFactory.Get(allowBulkExecution);
        var container = cosmosClient.GetContainer(
            _cosmosDbOptions.DatabaseName,
            VehicleTaxonomyContainerDefinition.Instance.ContainerName
            );

        return container;
    }

    private static string CreateParentPath(
        string? parentMakePublicId,
        string? parentModelPublicId
        )
    {
        if (string.IsNullOrEmpty(parentMakePublicId))
        {
            return "/";
        }

        if (string.IsNullOrEmpty(parentModelPublicId))
        {
            return $"/{parentMakePublicId}";
        }

        return $"/{parentMakePublicId}/{parentModelPublicId}";
    }

    private static string CreateItemId(
        string parentPath,
        string publicId
        )
    {
        var fullPath = $"{parentPath}/{publicId}";
        var bytes = Encoding.UTF8.GetBytes(fullPath);

        // hash and encode as it is recommended to only use alphanumeric characters for full compatibility with other systems
        var hash = SHA1.HashData(bytes);
        var id = _sqidsEncoder.Encode(hash);

        return id;
    }
}
