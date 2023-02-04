using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;

using WebTesting.Shared;

using Xunit.Abstractions;

namespace WebTesting.ProductTests;

[Collection(DefaultDatabaseCollection.Name)]
public sealed class TodoControllerTests :
    IClassFixture<IntegrationTestFactory>,
    IDisposable
{
    private readonly IntegrationTestFactory _factory;
    private readonly ITestOutputHelper _output;
    private readonly HttpClient _client;

    public TodoControllerTests(
        IntegrationTestFactory factory,
        ITestOutputHelper output)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(output);

        _factory = factory;
        _output = output;
        factory.Log = output.WriteLine;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task GetTodos_returns_some_todo_data()
    {
        var values = await _client.GetFromJsonAsync<IList<TodoItem>>("api/todos").ConfigureAwait(false);
        _output.WriteLine("Successfully read {0} items from the server.", values.Count);
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
    }
}
