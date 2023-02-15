using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

namespace WebTesting.BehavioralTests.Steps;

[Binding]
public sealed class TodoManagementSteps :
    IDisposable,
    IAsyncLifetime,
    IClassFixture<CancellationTokenFixture>,
    IClassFixture<ServicesFixture>,
    IClassFixture<RespawnFixture>
{
    private readonly TodoBuilder _builder = new();
    private readonly CancellationTokenFixture _cancellation;
    private readonly ServicesFixture _services;
    private readonly RespawnFixture _respawner;
    private readonly ITestOutputHelper _output;
    private readonly IServiceScope _scope;
    private TodoItem? _todoItem;

    public TodoManagementSteps(
        CancellationTokenFixture cancellation,
        ServicesFixture services,
        RespawnFixture respawner,
        ITestOutputHelper output)
    {
        ArgumentNullException.ThrowIfNull(cancellation);
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(respawner);
        ArgumentNullException.ThrowIfNull(output);

        _cancellation = cancellation;
        _services = services;
        _respawner = respawner;
        _output = output;
        _services.Log = output.WriteLine;

        _scope = services.ServiceProvider.CreateScope();
    }

    public async Task InitializeAsync()
    {
        await _respawner.ResetAsync().ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Given("a todo with title '([^']+)'")]
    public void ATodoWithTitle(string title)
    {
        title.Should().NotBeNullOrWhiteSpace("the todo title is required");
        _builder.title = title;
    }

    [Given("the item '([^']+)'")]
    public void TheItem(string item)
    {
        item.Should().NotBeNullOrEmpty("item details are required");
        _builder.items.Add(item);
    }

    [Given("an id of '([a-f0-9]{32})'")]
    public void AnIdOf(string id)
    {
        _builder.id = Guid.Parse(id);
    }

    [When("the todo does not exist")]
    public async Task TheTodoDoesNotExist()
    {
        _output.WriteLine("When: the todo does not exist ({0})", _builder.title);
        var todoService = _scope.ServiceProvider.GetRequiredService<ITodoService>();

        var items = await todoService.GetItemsAsync(_cancellation.Token).ConfigureAwait(false);

        var todo = items.FirstOrDefault(todo => string.Equals(todo.Title, _builder.title, StringComparison.OrdinalIgnoreCase));
        todo.Should().BeNull();
    }

    [When("the todo was created")]
    public async Task TheTodoWasCreated()
    {
        var id = _builder.id;
        id.Should().NotBeNull("this step depends on the id being set");

        var sut = _scope.ServiceProvider.GetRequiredService<ITodoService>();
        var todoItem = new TodoItem()
        {
            Title = _builder.title,
            Details = _builder.items,
            Created = DateTimeOffset.Now,
            Id = id!.Value,
            Revision = 1,
        };

        _todoItem = await sut.CreateOrUpdateAsync(todoItem, _cancellation.Token).ConfigureAwait(false);
    }

    [Then("the todo is created with revision #(\\d+)")]
    public async Task TheTodoIsCreated(int revision)
    {
        var sut = _scope.ServiceProvider.GetRequiredService<ITodoService>();
        var todo = new TodoItem()
        {
            Title = _builder.title,
            Details = _builder.items,
            Created = DateTime.Now,
            Id = Guid.NewGuid(),
            Revision = revision
        };

        var result = await sut.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);
        Assert.NotNull(result);
        result.Revision.Should().Be(revision);
    }

    [Then("the todo is removed")]
    public async Task TheTodoIsRemoved()
    {
        var todoItem = _todoItem;
        todoItem.Should().NotBeNull("The todoItem should have been created in a previous step");
        var sut = _scope.ServiceProvider.GetRequiredService<ITodoService>();
        await sut.DeleteAsync(todoItem!.Id, _cancellation.Token).ConfigureAwait(false);
    }
}
