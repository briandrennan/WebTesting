using System.Text.Json;

using Microsoft.Extensions.Options;

namespace WebTesting.Client;

public class TodoHttpClient
{
    public TodoHttpClient(HttpClient httpClient, IOptions<TodoClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        HttpClient = httpClient;
        httpClient.BaseAddress = options.Value.BaseAddress;
        JsonSerializerOptions = options.Value.JsonSerializerOptions;
    }

    public HttpClient HttpClient { get; }

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
