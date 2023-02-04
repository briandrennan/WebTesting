using Microsoft.Extensions.Configuration;

namespace WebTesting.ProductTests;
internal static class TestSettingsFile
{
    internal static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("testsettings.json", false)
        .Build();

    internal static readonly string ConnectionString = Configuration.GetConnectionString("Database")!;
}
