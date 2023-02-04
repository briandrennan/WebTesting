using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using WebTesting.Client;
using WebTesting.Shared;

namespace WebTesting.ProductTests;

public sealed class TodoClientRegistrationTests
{
    [Fact]
    public void Registration_adds_expected_members()
    {
        var myOptions = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
        var baseAddress = new Uri("http://localhost:7979/", UriKind.Absolute);

        var services = new ServiceCollection()
            .AddTodoClient(client =>
            {
                client.BaseAddress = baseAddress;
                client.JsonSerializerOptions = myOptions;
            });

        using var sp = services.BuildServiceProvider();
        _ = sp.GetRequiredService<ITodoService>();
        var client = sp.GetRequiredService<HttpTodoService>();

        Assert.Equal(baseAddress, client.HttpClient.BaseAddress);
        Assert.Equal(myOptions, client.Options);
    }
}
