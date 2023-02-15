using Microsoft.Extensions.Configuration;

namespace WebTesting.Contrib;
public static class TestSettingsFile
{
    public static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("testsettings.json", false)
        .Build();

    public static readonly string ConnectionString = Configuration.GetConnectionString("Database")!;

    public static readonly bool EnableLogging =
        bool.TryParse(Configuration.GetSection("EnableLogging").Value, out var enableLogging)
        && enableLogging;
}
