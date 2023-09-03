namespace WebTesting.Contrib;

[CollectionDefinition(Name)]
public class DefaultDatabaseCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
    public const string Name = $"Default {nameof(DatabaseFixture)}";
}
