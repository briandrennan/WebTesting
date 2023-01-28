namespace WebTesting.Core.EFCore;

public class TodoDetail
{
    public long TodoDetailId { get; set; }

    public long TodoId { get; set; }

    public string Detail { get; set; } = string.Empty;

    public int Revision { get; set; }

    public virtual Todo Todo { get; set; } = null!;
}
