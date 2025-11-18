using AllinOne.MemoryCache.Interfaces;
using AllinOne.Middlewares;
using Microsoft.Extensions.Caching.Memory;

namespace AllinOne.MemoryCache.Implementations
{
    public class CustomCacheKeyHandler : ICustomCacheKeyHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CustomCacheKeyHandler> _logger;

        public CustomCacheKeyHandler(ILogger<CustomCacheKeyHandler> logger,IMemoryCache cache)
        {
            _memoryCache = cache;
            _logger = logger;

        }
        public T? StoreInMemoryAbsoluteCustomKey<T>(string key, T obj, double secondsToLive = 120) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            T? result = null;

            try
            {
                result = _memoryCache.Set(key, obj,
                    new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(secondsToLive),
                        Size = 1
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while trying to store in memory with absolute key {key}." + ex.ToString());
            }

            return result;
        }

        public T? StoreInMemorySlidingCustomKey<T>(string key, T obj, double secondsToLive = 120) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            T? result = null;

            try
            {
                result = _memoryCache.Set(key, obj,
                    new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(secondsToLive),
                        Size = 1
                    });

            }
            catch (Exception e)
            {
               // _logger.LogException(nameof(StoreInMemorySlidingCustomKey), e, $"Exception while trying to store in memory with sliding key {key}");
            }

            return result;
        }

        public bool TryRetrieveKey<T>(string key, out T? value) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            value = null;
            try
            {
                return _memoryCache.TryGetValue(key, out value);
            }
            catch (Exception e)
            {
                //_logger.LogException(nameof(TryRetrieveKey), e, $"Exception while trying to retrieve key {key} from memory");
                value = null;
                return false;
            }
        }

        public T? GetFromMemoryCustomKey<T>(string key) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                return _memoryCache.TryGetValue(key, out T? value) ? value : null;
            }
            catch (Exception e)
            {
                //_logger.LogException(nameof(GetFromMemoryCustomKey), e, $"Exception while trying to get key {key} from memory");

            }
            return null;
        }

        public void RemoveFromMemoryCustomKey(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                _memoryCache.Remove(key);
            }
            catch (Exception e)
            {
                //_logger.LogException(nameof(RemoveFromMemoryCustomKey), e, $"Exception while trying to remove key {key} from memory");
            }
        }

    }
}
