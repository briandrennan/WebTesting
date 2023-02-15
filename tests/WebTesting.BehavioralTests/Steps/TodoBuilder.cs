namespace WebTesting.BehavioralTests.Steps;

internal sealed class TodoBuilder
{
    internal string title = string.Empty;
    internal List<string> items = new();
    internal Guid? id;
}
