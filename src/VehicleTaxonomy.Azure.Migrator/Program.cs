using Microsoft.Extensions.DependencyInjection;
using VehicleTaxonomy.Azure.Infrastructure.Db;
using VehicleTaxonomy.Azure.Infrastructure.Files;
using VehicleTaxonomy.Azure.Migrator;

// LocalInfraInitializer?
Console.WriteLine("Initializing Infra");

var rootServiceProvider = HostServiceProvider.CreateServiceProvider();

using var scope = rootServiceProvider.CreateScope();

var initializer = scope.ServiceProvider.GetRequiredService<CosmosDbContainerInitializer>();
Console.WriteLine("Initializing Cosmos");
await initializer.CreateIfNotExistsAsync(VehicleTaxonomyContainerDefinition.Instance);

var blobClientFactory = scope.ServiceProvider.GetRequiredService<BlobClientFactory>();
var blobClient = blobClientFactory.GetContainerClient(TaxonomyImportBlobContainer.ContainerName);

Console.WriteLine("Initializing BlobStorage");
await blobClient.CreateIfNotExistsAsync();

Console.WriteLine("Done");
