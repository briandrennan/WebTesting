namespace WebTesting.Shared;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItem>> GetItemsAsync(CancellationToken cancellationToken);

    Task<TodoItem> CreateOrUpdateAsync(TodoItem todo, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
