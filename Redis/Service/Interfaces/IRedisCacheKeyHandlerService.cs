namespace AllinOne.Redis.Service.Interfaces
{
    public interface IRedisCacheKeyHandlerService
    {
        Task<bool> StoreOrUpdateRedisExpirationCustomKeyAsync<T>(string key, T obj, double secondsToLive = 120) where T : class;
        Task<T?> GetFromRedisCustomKeyAsync<T>(string key) where T : class;
        Task<(bool Found, T? Value)> TryRetrieveKeyAsync<T>(string key) where T : class;
        Task RemoveFromRedisCustomKeyAsync(string key);
        Task RefreshExpirationRedisKeyAsync(string key);
        Task<bool> StoreOrUpdateInRedisPermanentKeyAsync<T>(string key, T obj) where T : class;
    }
}
