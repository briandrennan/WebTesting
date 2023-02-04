using Microsoft.Extensions.Logging;

namespace WebTesting.ProductTests;
internal sealed class TestMessageLogger : ILogger
{
    private readonly ServicesFixture _fixture;
    private readonly string _categoryName;

    public TestMessageLogger(ServicesFixture fixture, string categoryName)
    {
        _fixture = fixture;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return bool.TryParse(_fixture.Configuration.GetSection("EnableLogging").Value, out var isEnabled)
            && isEnabled;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _fixture.Log?.Invoke(formatter(state, exception));
    }
}
