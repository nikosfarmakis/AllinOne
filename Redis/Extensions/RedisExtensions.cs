
using AllinOne.Models.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AllinOne.Redis.Extensions
{
    public static class RedisExtensions
    {
        //singleton to avoid opening multiple connections and have consistent access throughout the application
        //StackExchange.Redis manages connection pooling
        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            services.AddSingleton<IDistributedCache>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RedisSection>>().Value;

                if (!settings.Enabled)
                {
                    return null;
                }

                return new RedisCache(new RedisCacheOptions
                {
                    Configuration = settings.Configuration,
                    InstanceName = settings.InstanceName
                });
            });

            // Register IConnectionMultiplexer for low-level access Redis operations AdminService
            // Connection pooling is done inside the multiplexer
            // singleton to avoid opening multiple connections and have consistent access throughout the application
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RedisSection>>().Value;

                if (!settings.Enabled)
                {
                    return null;
                }

                var configOptions = ConfigurationOptions.Parse(settings.Configuration);
                configOptions.Ssl = settings.Ssl;
                configOptions.AbortOnConnectFail = settings.AbortOnConnectFail!.Value; ;
                configOptions.ConnectTimeout = settings.ConnectTimeout!.Value; ;
                configOptions.SyncTimeout = settings.SyncTimeout!.Value; ;

                return ConnectionMultiplexer.Connect(configOptions);
            });

            return services;
        }
    }
}
