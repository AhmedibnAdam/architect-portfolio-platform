namespace Backend.IntegrationTests.Persistence;

[CollectionDefinition(nameof(PostgresCollection))]
public sealed class PostgresCollection : ICollectionFixture<PostgresDatabaseFixture>
{
}
