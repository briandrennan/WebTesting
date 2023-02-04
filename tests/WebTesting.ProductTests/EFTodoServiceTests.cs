using Microsoft.EntityFrameworkCore;

using WebTesting.Core;
using WebTesting.Core.EFCore;
using WebTesting.Shared;

using Xunit.Abstractions;

namespace WebTesting.ProductTests;

[Collection(DefaultDatabaseCollection.Name)]
public sealed class EFTodoServiceTests :
    IClassFixture<TodoContextFixture>,
    IClassFixture<RespawnFixture>,
    IClassFixture<CancellationTokenFixture>
{
    private readonly TodoContext _dbContext;
    private readonly RespawnFixture _respawner;
    private readonly CancellationTokenFixture _cancellation;
    private readonly EFTodoService _sut;
    private readonly ITestOutputHelper _output;

    public EFTodoServiceTests(
        TodoContextFixture todoContext,
        RespawnFixture respawner,
        CancellationTokenFixture cancellation,
        ITestOutputHelper output)
    {
        ArgumentNullException.ThrowIfNull(todoContext);
        ArgumentNullException.ThrowIfNull(respawner);
        ArgumentNullException.ThrowIfNull(cancellation);
        ArgumentNullException.ThrowIfNull(output);

        _dbContext = todoContext.Context;
        _sut = new(_dbContext);
        _respawner = respawner;
        _cancellation = cancellation;
        _output = output;
        todoContext.Log = output.WriteLine;
    }

    [Fact]
    public async Task GetItems_IsEmpty()
    {
        await _respawner.ResetAsync().ConfigureAwait(false);
        var result = await _sut.GetItemsAsync(_cancellation.Token).ConfigureAwait(false);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("716363DB-08AA-48DE-AFE6-FA49AD78EB2D")]
    public async Task Todo_e2e(string todoId)
    {
        await _respawner.ResetAsync().ConfigureAwait(false);
        var todo = new TodoItem()
        {
            Id = Guid.Parse(todoId),
            Revision = 1,
            Title = nameof(Todo_e2e),
            Details = new List<string>()
            {
                "Check code coverage",
                "Add item list",
                "Update item list"
            }
        };

        var result = await _sut.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);

        var item = _dbContext
            .Todos
            .TagWith("Re-hydrate persisted data")
            .Include(e => e.Details)
            .First(e => e.Id == result.Id);

        Assert.NotNull(item);
        foreach (var detail in item.Details)
        {
            Assert.Contains(detail.Detail, todo.Details);
        }

        todo = todo with
        {
            Id = result.Id,
            Revision = todo.Revision,
            Details = new List<string>()
            {
                "Check code coverage",
                "Test update method"
            }
        };
        result = await _sut.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);

        item = _dbContext
            .Todos
            .TagWith("Re-hydrate persisted data")
            .Include(e => e.Details)
            .First(e => e.Id == todo.Id);

        Assert.NotNull(item);
        Assert.Equal(2, item.Details.Count);
        Assert.Equal(todo.Details.First(), item.Details.First().Detail);
        Assert.Equal(todo.Details.Last(), item.Details.Last().Detail);

        await _sut.DeleteAsync(todo.Id, _cancellation.Token).ConfigureAwait(false);
        item = _dbContext.Todos.FirstOrDefault(e => e.TodoId == item.TodoId);
        Assert.Null(item);
    }

    [Fact]
    public async Task CreateOrUpdate_Todo_with_expired_revision_fails()
    {
        await _respawner.ResetAsync().ConfigureAwait(false);
        var todo = new TodoItem()
        {
            Id = Guid.Parse("9D875342-4BEF-4557-AF38-50488C6E57C0"),
            Title = nameof(Todo_e2e),
            Details = new List<string>()
            {
                "create todo",
                "try update",
                "assert failed for revision not changing"
            }
        };

        var item = await _sut.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);

        Assert.NotNull(item);
        todo = todo with { Revision = item.Revision + 1 };
        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await _sut.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
