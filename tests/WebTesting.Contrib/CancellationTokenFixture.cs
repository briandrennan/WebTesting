using System.Diagnostics;

namespace WebTesting.Contrib;

public sealed class CancellationTokenFixture : IDisposable
{
    public CancellationTokenFixture()
    {
        Source = new CancellationTokenSource();
        TimeSpan delay = Debugger.IsAttached ? TimeSpan.FromMinutes(10) : TimeSpan.FromSeconds(10);
        Source.CancelAfter(delay);
    }

    public CancellationTokenSource Source { get; }

    public CancellationToken Token => Source.Token;

    public void Dispose()
    {
        Source.Dispose();
        GC.SuppressFinalize(this);
    }
}
