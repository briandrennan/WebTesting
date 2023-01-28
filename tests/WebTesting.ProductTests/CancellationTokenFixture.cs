using System.Diagnostics;

namespace WebTesting.ProductTests;

public sealed class CancellationTokenFixture : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public CancellationTokenFixture()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        TimeSpan delay = Debugger.IsAttached ? TimeSpan.FromMinutes(10) : TimeSpan.FromSeconds(10);
        _cancellationTokenSource.CancelAfter(delay);
    }

    public CancellationToken Token => _cancellationTokenSource.Token;

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}
