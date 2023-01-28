using WebTesting.Core.EFCore;
using WebTesting.Shared;

namespace WebTesting.Core;

public sealed class EFTodoService : ITodoService
{
    private readonly TodoContext _context;

    public EFTodoService(TodoContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<TodoItem> CreateOrUpdateAsync(TodoItem todo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(todo);
        if (todo.Id == Guid.Empty)
        {
            todo = todo with { Id = Guid.NewGuid() };
        }

        var entry = await _context.Todos
            .TagWithCallSite()
            .Include(t => t.Details)
            .AsTracking()
            .Where(e => e.Id == todo.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (entry is null)
        {
            entry = new Todo()
            {
                Created = DateTimeOffset.Now,
                Details = todo.Details.Select(e => new TodoDetail
                {
                    Revision = 1,
                    Detail = e,
                }).ToList(),
                Revision = 1,
                Id = todo.Id,
                Title = todo.Title,
            };
            _context.Todos.Add(entry);
        }
        else if (todo.Revision > 0 && entry.Revision != todo.Revision)
        {
            throw new InvalidOperationException($"The revision {todo.Revision} specified has expired. The current revision is {entry.Revision}.");
        }
        else
        {
            entry.Revision++;
            var set = new HashSet<string>(todo.Details, StringComparer.OrdinalIgnoreCase);
            foreach (var item in entry.Details)
            {
                if (!set.Contains(item.Detail))
                {
                    _context.Details.Remove(item);
                }
                else
                {
                    item.Revision = entry.Revision;
                }
            }

            var newEntries = set.Except(entry.Details.Select(e => e.Detail))
                .Select(e => new TodoDetail
                {
                    Detail = e,
                    Revision = entry.Revision,
                    Todo = entry,
                });
            _context.Details.AddRange(newEntries);
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new TodoItem
        {
            Created = entry.Created,
            Details = entry.Details.Select(e => e.Detail),
            Id = entry.Id,
            Title = entry.Title,
            Revision = entry.Revision,
        };
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Todos
            .TagWithCallSite()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TodoItem>> GetItemsAsync(CancellationToken cancellationToken)
    {
        return await _context.Todos
            .TagWithCallSite()
            .Select(e => new TodoItem
            {
                Created = e.Created,
                Details = e.Details.Select(e => e.Detail),
                Id = e.Id,
                Title = e.Title,
                Revision = e.Revision,
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
