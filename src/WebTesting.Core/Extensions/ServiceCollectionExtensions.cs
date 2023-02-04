using Microsoft.Extensions.DependencyInjection;

using WebTesting.Core.EFCore;
using WebTesting.Shared;

namespace WebTesting.Core.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextPool<TodoContext>(options =>
        {
            options.EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            options.UseSqlServer(connectionString, sqlServer =>
            {
                sqlServer.CommandTimeout(60)
                    .EnableRetryOnFailure()
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        services
            .AddScoped<ITodoService, EFTodoService>();

        return services;
    }
}
