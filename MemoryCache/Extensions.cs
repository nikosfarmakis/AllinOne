using AllinOne.MemoryCache.Implementations;
using AllinOne.MemoryCache.Interfaces;

namespace AllinOne.MemoryCache
{
    public static class Extensions
    {
        public static void AddCustomCacheMemoryKeysHandler(this IServiceCollection services)
        {
            services.AddMemoryCache(context =>
            {
                context.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
                context.SizeLimit = 1000;
                context.CompactionPercentage = 0.05;
            });
            services.AddSingleton<ICustomCacheKeyHandler, CustomCacheKeyHandler>();
        }
    }
}
