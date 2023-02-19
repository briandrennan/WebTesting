using System.Net.Http.Headers;

using Microsoft.Extensions.DependencyInjection;

using WebTesting.Shared;

namespace WebTesting.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoClient(this IServiceCollection services, Action<TodoClientOptions> options)
    {
        services.AddOptions<TodoClientOptions>()
            .Configure(options)
            .Validate(opt => opt.BaseAddress is not null, "The BaseAddress must be configured.")
            .Validate(opt => opt.BaseAddress.IsAbsoluteUri, "The BaseAddress must be an absolute Uri.")
            .ValidateOnStart()
            ;

        services.AddHttpClient<TodoHttpClient>(httpClient =>
        {
            httpClient.BaseAddress = null;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services
            .AddTransient<ITodoService, HttpTodoService>()
            .AddTransient<HttpTodoService>()
            ;
    }
}
