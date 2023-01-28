using Microsoft.EntityFrameworkCore;

using WebTesting.Core.EFCore;

namespace WebTesting.ProductTests;

[Collection(nameof(DefaultDatabaseCollection.Name))]
public sealed class EFTodoServiceTests : IClassFixture<DatabaseFixture>, IClassFixture<CancellationTokenFixture>
{
    private readonly TodoContext _database;
    private readonly CancellationTokenFixture _cancellation;

    public EFTodoServiceTests(DatabaseFixture database, CancellationTokenFixture cancellation)
    {
        ArgumentNullException.ThrowIfNull(database);

        _database = database.Context;
        _cancellation = cancellation;
    }

    [Fact]
    public async Task GetItems_IsEmpty_Or_NonZero_TestAsync()
    {
        var context = _database;
        var result = await context.Todos.ToListAsync(_cancellation.Token).ConfigureAwait(false);
        Assert.NotNull(result);
    }
}
