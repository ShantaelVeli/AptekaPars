using Serilog;

namespace Configurator.serialog
{
    public class serialogConfig
    {
        public static void ConfigureLogger()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        }
    }
}