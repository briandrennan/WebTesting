using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using WebTesting.Core.Extensions;

namespace WebTesting.ProductTests;

public sealed class ServicesFixture : IDisposable, ILoggable
{
    public ServicesFixture()
    {
        CustomConfig = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var memorySource = new MemoryConfigurationSource
        {
            InitialData = CustomConfig
        };

        Configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Add(memorySource)
            .Build();

        string? connectionString = Configuration.GetConnectionString("Database");
        if (connectionString is null)
            throw new InvalidOperationException("The 'Database' connection string is missing.");

        ServiceProvider = new ServiceCollection()
            .AddSingleton(Configuration)
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new TestMessageLoggerProvider(this));
            })
            .AddTodoServices(connectionString)
            .BuildServiceProvider();
    }

    public string ConnectionString => Configuration.GetConnectionString("Database")!;

    public Dictionary<string, string?> CustomConfig { get; }

    public IConfiguration Configuration { get; }

    public IServiceProvider ServiceProvider { get; }

    public Action<string>? Log { get; set; }

    public void Dispose()
    {
        (ServiceProvider as IDisposable)?.Dispose();
        (Configuration as IDisposable)?.Dispose();
    }
}
