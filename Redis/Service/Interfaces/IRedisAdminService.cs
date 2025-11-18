using AllinOne.ResultPattern;

namespace AllinOne.Redis.Service.Interfaces
{
    public interface IRedisAdminService
    {
        long GetTotalKeysCount();
        Task<InternalDataTransfer<Dictionary<string, string?>>> GetAllKeysAndValuesAsync();
        Task<(string Key, string? Value)> GetKeyAndValueAsync(string key);
        Task FlushAllDataAsync();
        Task<bool> DeleteKeyAsync(string key);
        Dictionary<string, Dictionary<string, string>> GetServerInfo();
    }
}
