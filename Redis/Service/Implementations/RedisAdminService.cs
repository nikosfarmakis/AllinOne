using AllinOne.Redis.Service.Interfaces;
using AllinOne.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System.Text.Json;

namespace AllinOne.Redis.Service.Implementations
{
    public class RedisAdminService : IRedisAdminService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<RedisAdminService> _logger;

        public RedisAdminService(IConnectionMultiplexer redis, ILogger<RedisAdminService> logger)
        {
            _redis = redis;
            _db = redis.GetDatabase();
            _logger = logger;
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints().First();
            return _redis.GetServer(endpoint);
        }

        public long GetTotalKeysCount()
        {
            var server = GetServer();
            return server.DatabaseSize();
        }

        public async Task<InternalDataTransfer<Dictionary<string, string?>>> GetAllKeysAndValuesAsync()
        {
            try
            {
                var result = new Dictionary<string, string?>();
                foreach (var endpoint in _redis.GetEndPoints())
                {
                    var server = _redis.GetServer(endpoint);

                    await foreach (var key in server.KeysAsync(pattern: "*", pageSize: 100))
                    {
                        string keyName = key.ToString();
                        try
                        {
                            var keyType = await _db.KeyTypeAsync(key);

                            switch (keyType)
                            {
                                /*                            case RedisType.String:
                                                                var stringValue = await _db.StringGetAsync(key);
                                                                result[keyName] = stringValue.HasValue ? stringValue.ToString() : "(null)";
                                                                break;*/

                                case RedisType.Hash:
                                    var hashEntries = await _db.HashGetAllAsync(key);
                                    result[keyName] = string.Join(", ",
                                        hashEntries.Select(h => $"{h.Name}={h.Value}"));
                                    break;

                                /*                            case RedisType.List:
                                                                var listLength = await _db.ListLengthAsync(key);
                                                                var listValues = await _db.ListRangeAsync(key, 0, Math.Min(listLength - 1, 10));
                                                                result[keyName] = $"[List] ({listLength} items) " +
                                                                    string.Join(", ", listValues.Select(v => v.ToString()));
                                                                break;

                                                            case RedisType.Set:
                                                                var setValues = await _db.SetMembersAsync(key);
                                                                result[keyName] = $"[Set] ({setValues.Length} items) " +
                                                                    string.Join(", ", setValues.Select(v => v.ToString()));
                                                                break;

                                                            case RedisType.SortedSet:
                                                                var zsetValues = await _db.SortedSetRangeByRankWithScoresAsync(key, 0, 9);
                                                                result[keyName] = $"[SortedSet] ({zsetValues.Length} items) " +
                                                                    string.Join(", ", zsetValues.Select(v => $"{v.Element}({v.Score})"));
                                                                break;*/

                                default:
                                    result[keyName] = $"[{keyType}] (unsupported type)";
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error retrieving key {Key}", keyName);
                            result[keyName] = "Error retrieving value";
                        }
                    }
                }
                _logger.LogInformation("Retrieved {Count} Redis keys successfully", result.Count);
                return new InternalDataTransfer<Dictionary< string, string?>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(GetAllKeysAndValuesAsync));
                return new InternalDataTransfer<Dictionary<string, string?>>(ex, $"Failed to retrieve Redis keys: {ex.Message}");
            }
        }

        /*        public async Task<()> GetPagedKeys(string? cursor = "0", int count = 50)
                {

                }*/


        /*        public async Task<IActionResult> GetPagedKeys([FromQuery] string? cursor = "0", [FromQuery] int count = 50)
                {
                    var server = _redis.GetServer(_redis.GetEndPoints().First());

                    var scanResult = server.Execute("SCAN", cursor, "COUNT", count);
                    var resultArray = (RedisResult[])scanResult;

                    string nextCursor = (string)resultArray[0];
                    List<string?> keys = ((RedisResult[])resultArray[1]).Select(k => (string)k).ToList();

                    return Ok(new
                    {
                        cursor = nextCursor,
                        hasMore = nextCursor != "0",
                        keys
                    });
                }*/


        public async Task<(string Key, string? Value)> GetKeyAndValueAsync(string key)
        {
            try
            {
                var type = await _db.KeyTypeAsync(key);
                string? valueResult;

                switch (type)
                {
                    /*                    case RedisType.String:
                                            var value = await _db.StringGetAsync(key);
                                            return (key, value.HasValue ? value.ToString() : null);*/

                    case RedisType.Hash:
                        var hashEntries = await _db.HashGetAllAsync(key);
                        var dict = hashEntries.ToDictionary(
                            e => e.Name.ToString(),
                            e => e.Value.ToString()
                        );
                        _logger.LogInformation("Retrieved Redis data Τype: {Type} Key: {Key}", type, key);
                        return (key, JsonSerializer.Serialize(dict));

                    /*                    case RedisType.List:
                                            var listValues = await _db.ListRangeAsync(key);
                                            return (key, JsonSerializer.Serialize(listValues.Select(v => v.ToString())));

                                        case RedisType.Set:
                                            var setValues = await _db.SetMembersAsync(key);
                                            return (key, JsonSerializer.Serialize(setValues.Select(v => v.ToString())));

                                        case RedisType.SortedSet:
                                            var zsetValues = await _db.SortedSetRangeByRankAsync(key);
                                            return (key, JsonSerializer.Serialize(zsetValues.Select(v => v.ToString())));*/

                    default:
                        _logger.LogWarning("Unsupported Redis data Τype: {Type} Key: {Key}", type, key);
                        return (key, $"[{type}] Unsupported Redis data type");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(GetKeyAndValueAsync));
                return (key, $"Error retrieving key: {ex.Message}");
            }
        }

        public async Task FlushAllDataAsync()
        {
            try
            {
                var server = GetServer();
                await server.FlushDatabaseAsync();
                _logger.LogWarning("All Redis data deleted via FlushAllDataAsync()");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(FlushAllDataAsync));
            }
        }
        public async Task<bool> DeleteKeyAsync(string key)
        {
            try
            {
                bool deleted = await _db.KeyDeleteAsync(key);
                if (deleted)
                    _logger.LogInformation("Deleted Key: {Key} from Redis", key);
                else
                    _logger.LogWarning("Tried to delete Key: {Key} but it was not found", key);

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(DeleteKeyAsync));
                return false;
            }
        }
        public Dictionary<string, Dictionary<string, string>>? GetServerInfo()
        {
            try
            {
                var server = GetServer();
                var info = server.Info();

                var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var group in info)
                {
                    result[group.Key] = group
                        .GroupBy(x => x.Key)
                        .ToDictionary(g => g.Key, g => g.First().Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(GetServerInfo));
                return null;
            }
            
        }
    }
}
