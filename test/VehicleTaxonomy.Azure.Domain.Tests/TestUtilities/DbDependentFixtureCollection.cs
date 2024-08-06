namespace VehicleTaxonomy.Azure.Domain.Tests;

[CollectionDefinition(nameof(DbDependentFixtureCollection))]
public class DbDependentFixtureCollection : ICollectionFixture<DbDependentFixture>
{
}
