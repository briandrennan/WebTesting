namespace WebTesting.Shared;

public sealed record TodoItem
{
    public Guid Id { get; init; } = Guid.Empty;

    public required string Title { get; init; }

    public required IEnumerable<string> Details { get; init; }

    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;

    public int Revision { get; init; } = -1;
}
