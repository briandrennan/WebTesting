using System.Net.Http.Json;
using System.Text.Json;

using WebTesting.Shared;

namespace WebTesting.Client;

public class HttpTodoService : ITodoService
{
    private static readonly Uri _todos = new("api/todos", UriKind.Relative);

    public HttpTodoService(TodoHttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        HttpClient = client.HttpClient;
        Options = client.JsonSerializerOptions;
    }

    public HttpClient HttpClient { get; }

    public JsonSerializerOptions? Options { get; }

    public async Task<TodoItem> CreateOrUpdateAsync(TodoItem todo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(todo);
        var response = await HttpClient.PutAsJsonAsync($"api/todos", todo, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TodoItem>(Options, cancellationToken)
            .ConfigureAwait(false);

        return result is not null ? result : throw new TodoClientException("Unknown response from server", response.StatusCode);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var todos = new Uri($"api/todos/{id}", UriKind.Relative);
        var response = await HttpClient.DeleteAsync(todos, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<TodoItem>> GetItemsAsync(CancellationToken cancellationToken)
    {
        var response = await HttpClient.GetAsync(_todos, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var resultCollection = await response.Content.ReadFromJsonAsync<IReadOnlyList<TodoItem>>(Options, cancellationToken)
            .ConfigureAwait(false);

        return resultCollection is not null ? resultCollection :
            throw new TodoClientException("Unknown response from server", response.StatusCode);
    }
}
