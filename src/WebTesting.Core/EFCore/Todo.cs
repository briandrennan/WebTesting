namespace WebTesting.Core.EFCore;

public class Todo
{
    public long TodoId { get; set; }

    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; }

    public int Revision { get; set; }

    public virtual ICollection<TodoDetail> Details { get; init; } = new HashSet<TodoDetail>();
}
