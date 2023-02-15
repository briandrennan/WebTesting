using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using WebTesting.Contrib;
using WebTesting.Core.EFCore;

using WebTestingApi;

namespace WebTesting.ProductTests;

public class IntegrationTestFactory : WebApplicationFactory<ApiMarker>, ILoggable
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureAppConfiguration(config =>
        {
            // Need to construct a full path to this file. The working directory isn't
            // used by default, which is somewhat frustrating.
            var testSettings = Path.Combine(Environment.CurrentDirectory, "testsettings.json");
            config.AddJsonFile(testSettings, optional: false);
        });

        builder.ConfigureServices(services =>
        {
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new TestMessageLoggerProvider(this));
            });

            var dbContextDescriptor = services.Single(
                d => d.ServiceType ==
                    typeof(DbContextOptions<TodoContext>));

            services.Remove(dbContextDescriptor);

            // Need to re-configure this so it uses our connection string.
            services.AddDbContextPool<TodoContext>((container, options) =>
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors();

                options.UseSqlServer(TestSettingsFile.ConnectionString, sqlServer =>
                {
                    sqlServer.CommandTimeout(60)
                        .EnableRetryOnFailure()
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });
        });
    }

    public Action<string>? Log { get; set; }
}
