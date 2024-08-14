using Microsoft.Azure.Cosmos;

namespace VehicleTaxonomy.Azure.Infrastructure.Db;

/// <summary>
/// Helper for initializing CosmosDb containers/collections. This is currently
/// intended mainly for standing up local test databases and only considers
/// schema configuration. For real databases we use IaC.
/// </summary>
public class CosmosDbContainerInitializer
{
    private readonly CosmosDbClientFactory _cosmosDbClientFactory;
    private readonly CosmosDbOptions _cosmosDbOptions;

    public CosmosDbContainerInitializer(
        CosmosDbClientFactory cosmosDbClientFactory,
        CosmosDbOptions cosmosDbOptions
        )
    {
        _cosmosDbClientFactory = cosmosDbClientFactory;
        _cosmosDbOptions = cosmosDbOptions;
    }

    public async Task CreateIfNotExistsAsync(
        ICosmosDbContainerDefinition definition
        )
    {
        var database = await CreateDbIfNotExists();

        var containerProperties = definition.GetContainerProperties();
        await database.CreateContainerIfNotExistsAsync(containerProperties);
    }

    public async Task RebuildAsync(
        ICosmosDbContainerDefinition definition
        )
    {
        var database = await CreateDbIfNotExists();

        var containerProperties = definition.GetContainerProperties();
        var result = await database.CreateContainerIfNotExistsAsync(containerProperties);

        if (result.StatusCode != System.Net.HttpStatusCode.Created)
        {
            await result.Container.DeleteContainerAsync();
            await database.CreateContainerIfNotExistsAsync(containerProperties);
        }
    }

    private async Task<Database> CreateDbIfNotExists()
    {
        var cosmosClient = _cosmosDbClientFactory.Get();
        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(_cosmosDbOptions.DatabaseName, 400);
        var database = databaseResponse.Database;

        return database;
    }
}
