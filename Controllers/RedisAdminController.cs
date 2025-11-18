using AllinOne.Redis.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using StackExchange.Redis;

namespace AllinOne.Redis.Controllers
{
    [FeatureGate("ShowRedisAdminController")]
    [ApiController]
    [Route("api/[controller]")]
    public class RedisAdminController : ControllerBase
    {
        private readonly IRedisAdminService _redisAdmin;

        public RedisAdminController(IRedisAdminService redisAdmin)
        {
            _redisAdmin = redisAdmin;
        }

        /// <summary>
        /// Get total number of keys in Redis
        /// </summary>
        [HttpGet("count")]
        public IActionResult GetTotalKeysCount()
        {
            var count = _redisAdmin.GetTotalKeysCount();
            return Ok(new { TotalKeys = count });
        }

        /// <summary>
        /// Get all keys with their values
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllKeysAndValues()
        {
            var all = await _redisAdmin.GetAllKeysAndValuesAsync();
            return Ok(all);
        }

        /// <summary>
        /// Get a single key with its value
        /// </summary>
        [HttpGet("get")]
        public async Task<IActionResult> GetKeyValue([FromQuery] string key)
        {
            var (Key, Value) = await _redisAdmin.GetKeyAndValueAsync(key);
            if (Value == null)
                return NotFound(new { Key, Message = "Key not found" });

            return Ok(new { Key, Value });
        }

        /// <summary>
        /// Delete a single key
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteKey([FromQuery] string key)
        {
            bool deleted = await _redisAdmin.DeleteKeyAsync(key);
            if (deleted)
                return Ok(new { Key = key, Message = "Deleted" });

            return NotFound(new { Key = key, Message = "Key not found" });
        }

        /// <summary>
        /// Delete all data in Redis database
        /// </summary>
        [HttpDelete("flush")]
        public async Task<IActionResult> FlushAllData()
        {
            await _redisAdmin.FlushAllDataAsync();
            return Ok(new { Message = "All Redis data deleted" });
        }

        /// <summary>
        /// Get server information and stats
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetServerInfo()
        {
            var info = _redisAdmin.GetServerInfo();
            return Ok(info);

        }
    }
}
