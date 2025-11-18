namespace AllinOne.MemoryCache.Interfaces
{
    public interface ICustomCacheKeyHandler
    {
        T? StoreInMemoryAbsoluteCustomKey<T>(string key, T obj, double secondsToLive = 120) where T : class;
        T? StoreInMemorySlidingCustomKey<T>(string key, T obj, double secondsToLive = 120) where T : class;
        bool TryRetrieveKey<T>(string key, out T? result) where T : class;
        T? GetFromMemoryCustomKey<T>(string key) where T : class;
        void RemoveFromMemoryCustomKey(string key);
    }
}
