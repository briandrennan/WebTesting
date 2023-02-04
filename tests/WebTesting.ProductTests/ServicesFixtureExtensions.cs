using Microsoft.Extensions.DependencyInjection;

using WebTesting.Core.EFCore;

namespace WebTesting.ProductTests;

internal static class ServicesFixtureExtensions
{
    public static TodoContext GetTodoContext(this ServicesFixture services)
        => services.ServiceProvider.GetRequiredService<TodoContext>();
}
