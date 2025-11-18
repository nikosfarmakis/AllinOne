using Microsoft.FeatureManagement;
using Serilog;

namespace AllinOne.Utils.Extensions
{
    public static class SerilogServiceExtensions
    {
        public static IServiceCollection AddSerilogWithFeatureFlags(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddFeatureManagement();

            using var provider = services.BuildServiceProvider();
            var features = provider.GetRequiredService<IFeatureManagerSnapshot>();

            // basic Serilog config
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName();

            var enableEfLogging = features.IsEnabledAsync("EFCoreWriteLoggingFile").Result;

            if (enableEfLogging)
            {
                loggerConfig.WriteTo.Logger(lc =>
                    lc.Filter.ByIncludingOnly("Contains(SourceContext, 'Microsoft.EntityFrameworkCore')")
                      .WriteTo.File(
                          path: "logs/efcore/efcore-.txt",
                          rollingInterval: RollingInterval.Day,
                          outputTemplate:
                              "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [Thread:{ThreadId}] ({SourceContext})" +
                              "{NewLine}[CorrelationId={CorrelationId}] {Message:lj}{NewLine}{Exception}"
                      )
                );
            }

            Log.Logger = loggerConfig.CreateLogger();

            return services;
        }
    }
}
