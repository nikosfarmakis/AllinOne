using AllinOne.Redis.Service.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AllinOne.Redis.Service.Implementations
{
    public class RedisCacheKeyHandlerService : IRedisCacheKeyHandlerService
    {
        private readonly IDistributedCache _redisCache; // IDistributedCache abstraction .NET
        private readonly ILogger<RedisCacheKeyHandlerService> _logger;

        public RedisCacheKeyHandlerService(ILogger<RedisCacheKeyHandlerService> logger, IDistributedCache redisCache)
        {
            _logger = logger;
            _redisCache = redisCache;
        }

        public async Task<bool> StoreOrUpdateRedisExpirationCustomKeyAsync<T>(
            string key, T obj, double secondsToLive = 120) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                var json = JsonSerializer.Serialize(obj);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(secondsToLive)
                };

                await _redisCache.SetStringAsync(key, json, options);
                _logger.LogInformation("Stored or Updated Expiration Custom Key {EntityName} successfully with Key: {Key}.",
                    typeof(T).Name, key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Key: {Key} in Redis.",
                    nameof(StoreOrUpdateRedisExpirationCustomKeyAsync), typeof(T).Name, key);
                return false;
            }
        }

        public async Task<T?> GetFromRedisCustomKeyAsync<T>(string key) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                var json = await _redisCache.GetStringAsync(key);
                var result = json is null ? null : JsonSerializer.Deserialize<T>(json);
                _logger.LogInformation("Retrieved {EntityName} successfully, {Entity}.", typeof(T).Name, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Key: {Key} in Redis.",
                    nameof(GetFromRedisCustomKeyAsync), typeof(T).Name, key);
                return null;
            }
        }

        public async Task<(bool Found, T? Value)> TryRetrieveKeyAsync<T>(string key) where T : class
        {
            var value = await GetFromRedisCustomKeyAsync<T>(key);
            return (value is not null, value);
        }

        public async Task RemoveFromRedisCustomKeyAsync(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                await _redisCache.RemoveAsync(key);
                _logger.LogInformation("Deleted successfully with Key: {Key}", key);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} with Key: {Key} in Redis.",
                    nameof(RemoveFromRedisCustomKeyAsync), key);
            }
        }

        public async Task RefreshExpirationRedisKeyAsync(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                await _redisCache.RefreshAsync(key);
                _logger.LogInformation("Refreshed successfully with Key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} with Key: {Key} in Redis.",
                    nameof(RefreshExpirationRedisKeyAsync), key);
            }
        }
        public async Task<bool> StoreOrUpdateInRedisPermanentKeyAsync<T>(string key, T obj) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            try
            {
                var json = JsonSerializer.Serialize(obj);

                await _redisCache.SetStringAsync(key, json);
                _logger.LogInformation("Stored or Updated Permanent Custom Key {EntityName} successfully with Key: {Key}.",
                    typeof(T).Name, key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Key: {Key} in Redis.",
                    nameof(StoreOrUpdateInRedisPermanentKeyAsync), typeof(T).Name, key);
                return false;
            }
        }
    }
}
