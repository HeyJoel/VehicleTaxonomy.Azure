namespace VehicleTaxonomy.Azure.Infrastructure.Files;

public class BlobStorageOptions
{
    /// <summary>
    /// Configuration section name in an appsettings.json
    /// file or similar.
    /// </summary>
    public const string SectionName = "BlobStorage";

    /// <summary>
    /// Used to connect and authenticate with the blob storage account. The storage
    /// account should not be the same as the function storage account as per official
    /// documented advice (perf reasons).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
