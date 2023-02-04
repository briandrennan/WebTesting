using System.Text.Json;

namespace WebTesting.Client;

public sealed class TodoClientOptions
{
    public Uri BaseAddress { get; set; } = null!;

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
