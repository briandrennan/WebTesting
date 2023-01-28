using Microsoft.AspNetCore.Mvc;

using WebTesting.Shared;

namespace WebTestingApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<TodoItem>> CreateOrUpdateAsync(
        [FromServices] ITodoService todoService,
        [FromBody] TodoItem todo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(todoService);
        return await todoService.CreateOrUpdateAsync(todo, cancellationToken).ConfigureAwait(false);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(
        [FromServices] ITodoService todoService,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(todoService);
        await todoService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
    }

    [HttpGet]
    public async Task<IReadOnlyList<TodoItem>> GetItemsAsync(
        [FromServices] ITodoService todoService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(todoService);
        return await todoService.GetItemsAsync(cancellationToken).ConfigureAwait(false);
    }
}
