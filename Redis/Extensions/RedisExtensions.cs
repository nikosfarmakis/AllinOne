
using StackExchange.Redis;

namespace AllinOne.Redis.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSection = configuration.GetSection("RedisSettings");
            var enabled = redisSection.GetValue<bool>("Enabled");

            if (!enabled)
            {
                return services; 
            }

            var connectionString = redisSection.GetValue<string>("Configuration");
            var instanceName = redisSection.GetValue<string>("InstanceName");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                options.InstanceName = instanceName;
            });

            // Register IConnectionMultiplexer for low-level Redis operations (AdminService)
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(connectionString));

            return services;
        }
    }
}
