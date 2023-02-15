using Microsoft.Extensions.Options;

using WebTesting.Client;
using WebTesting.Contrib;
using WebTesting.Shared;

using Xunit.Abstractions;

namespace WebTesting.ProductTests;

[Collection(DefaultDatabaseCollection.Name)]
public sealed class TodoEndToEndTests :
    IClassFixture<IntegrationTestFactory>,
    IClassFixture<CancellationTokenFixture>,
    IDisposable
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;
    private readonly HttpTodoService _todoClient;
    private readonly CancellationTokenFixture _cancellation;
    private readonly ITestOutputHelper _output;

    public TodoEndToEndTests(
        IntegrationTestFactory factory,
        CancellationTokenFixture cancellationTokenFixture,
        ITestOutputHelper output)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(cancellationTokenFixture);

        _factory = factory;
        _client = factory.CreateClient();
        factory.Log = output.WriteLine;
        _output = output;
        _cancellation = cancellationTokenFixture;

        var clientOptions = new TodoClientOptions()
        {
            BaseAddress = factory.Server.BaseAddress
        };
        var options = new OptionsWrapper<TodoClientOptions>(clientOptions);
        var typedClient = new TodoHttpClient(_client, options);
        _todoClient = new HttpTodoService(typedClient);
    }

    [Fact]
    public async Task Todo_end_to_end_test()
    {
        var todo = new TodoItem()
        {
            Created = DateTimeOffset.Now,
            Id = Guid.Parse("e7a527a9-794e-48f8-909f-6d7342b617ce"),
            Title = "Test GetOrCreateTodo integration",
            Revision = 1,
            Details = new string[]
            {
                "Call GetOrCreate on the server"
            }
        };

        var result = await _todoClient.CreateOrUpdateAsync(todo, _cancellation.Token).ConfigureAwait(false);
        Assert.NotNull(result);

        var reply = await _todoClient.GetItemsAsync(_cancellation.Token).ConfigureAwait(false);
        Assert.Contains(reply, val => val.Id == todo.Id);

        await _todoClient.DeleteAsync(todo.Id, _cancellation.Token).ConfigureAwait(false);
        reply = await _todoClient.GetItemsAsync(_cancellation.Token).ConfigureAwait(false);
        Assert.Equal(0, reply.Count);
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
    }
}
