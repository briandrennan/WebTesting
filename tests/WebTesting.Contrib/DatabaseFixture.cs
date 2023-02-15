using Microsoft.EntityFrameworkCore;

using WebTesting.Core.EFCore;

namespace WebTesting.Contrib;

[CollectionDefinition(Name)]
public class DefaultDatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = $"Default {nameof(DatabaseFixture)}";
}

public sealed class DatabaseFixture : IAsyncLifetime, IDisposable
{
    private readonly TodoContext _context;
    private bool _initialized;

    public DatabaseFixture(IMessageSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TodoDB;Integrated Security=True;";
        var options = new DbContextOptionsBuilder<TodoContext>();
        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        options.UseSqlServer(ConnectionString, sqlServer =>
        {
            sqlServer.CommandTimeout(60)
                .EnableRetryOnFailure();
        });

        _context = new TodoContext(options.Options);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync().ConfigureAwait(false);
    }

    public async Task InitializeAsync()
    {
        if (!_initialized)
        {
            await _context.Database.EnsureDeletedAsync().ConfigureAwait(false);
            await _context.Database.EnsureCreatedAsync().ConfigureAwait(false);
            _initialized = true;
        }
    }
}
