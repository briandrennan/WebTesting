using Microsoft.EntityFrameworkCore;

using WebTesting.Core.EFCore;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace WebTesting.ProductTests;

[Collection(DefaultDatabaseCollection.Name)]
public class DefaultDatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = $"Default {nameof(DatabaseFixture)}";
}

public class DatabaseFixture : IAsyncLifetime
{
    public DatabaseFixture(IMessageSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TodoDB;Integrated Security=True;";
        var options = new DbContextOptionsBuilder<TodoContext>();
        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(msg =>
            {
                var message = new DiagnosticMessage(msg);
                _ = sink.OnMessage(message);
            }, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information);

        options.UseSqlServer(ConnectionString, sqlServer =>
        {
            sqlServer.CommandTimeout(60)
                .EnableRetryOnFailure();
        });

        Context = new TodoContext(options.Options);
    }

    public TodoContext Context { get; }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync().ConfigureAwait(false);
    }

    public async Task InitializeAsync()
    {
        await Context.Database.EnsureDeletedAsync().ConfigureAwait(false);
        await Context.Database.EnsureCreatedAsync().ConfigureAwait(false);
    }
}
