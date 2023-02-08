using Microsoft.Extensions.Logging;

namespace WebTesting.ProductTests;

internal sealed class TestMessageLogger : ILogger
{
    private readonly ILoggable _fixture;

    public TestMessageLogger(ILoggable fixture, string categoryName)
    {
        _fixture = fixture;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
        => TestSettingsFile.EnableLogging;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
        => _fixture.Log?.Invoke(formatter(state, exception));
}
