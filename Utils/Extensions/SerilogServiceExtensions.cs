using Microsoft.FeatureManagement;
using Serilog;

namespace AllinOne.Utils.Extensions
{
    public static class SerilogServiceExtensions
    {
        public static IServiceCollection AddSerilogWithFeatureFlags(this IServiceCollection services, IConfiguration configuration)
        {
            // basic Serilog config
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName();

            Log.Logger = loggerConfig.CreateLogger();

            return services;
        }
    }
}
