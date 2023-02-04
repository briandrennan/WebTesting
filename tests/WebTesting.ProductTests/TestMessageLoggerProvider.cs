using Microsoft.Extensions.Logging;

namespace WebTesting.ProductTests;

internal sealed class TestMessageLoggerProvider : ILoggerProvider
{
    private readonly ServicesFixture _fixture;

    public TestMessageLoggerProvider(ServicesFixture fixture)
    {
        _fixture = fixture;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestMessageLogger(_fixture, categoryName);
    }

    public void Dispose()
    {
        // Nothing to do, required by the ILoggerProvider interface.
    }
}
