using Microsoft.EntityFrameworkCore;

using WebTesting.Core.EFCore;

namespace WebTesting.ProductTests;

public sealed class TodoContextFixture : IDisposable
{
    public TodoContextFixture()
    {
        var options = new DbContextOptionsBuilder<TodoContext>();
        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(LogMessage, minimumLevel: Microsoft.Extensions.Logging.LogLevel.Information);

        options.UseSqlServer(TestSettings.ConnectionString, sqlServer =>
        {
            sqlServer.CommandTimeout(60)
                .EnableRetryOnFailure();
        });

        Context = new TodoContext(options.Options);
    }

    public TodoContext Context { get; }

    public Action<string>? Log { get; set; }

    private void LogMessage(string message)
    {
        Log?.Invoke(message);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
