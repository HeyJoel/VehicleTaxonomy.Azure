namespace VehicleTaxonomy.Domain.Tests;

[CollectionDefinition(nameof(DbDependentFixtureCollection))]
public class DbDependentFixtureCollection : ICollectionFixture<DbDependentFixture>
{
}
