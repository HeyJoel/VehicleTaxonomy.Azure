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
}
