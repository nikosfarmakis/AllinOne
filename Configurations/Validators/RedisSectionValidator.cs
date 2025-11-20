using AllinOne.Models.Configuration;
using AllinOne.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class RedisSectionValidator : IValidateOptions<RedisSection>
    {
        public ValidateOptionsResult Validate(string name, RedisSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("RedisSettings cannot be null.");

            if (!options.Enabled)
                return ValidateOptionsResult.Success;

            if (options.Configuration.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("Redis Configuration must be provided when Redis is enabled.");

            if (options.InstanceName.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("Redis InstanceName must be provided when Redis is enabled.");

            if (options.AbortOnConnectFail == null)
                return ValidateOptionsResult.Fail("Redis AbortOnConnectFail must be defined when Redis is enabled.");

            if (options.ConnectTimeout == null)
                return ValidateOptionsResult.Fail("Redis ConnectTimeout must be defined when Redis is enabled.");

            if (options.ConnectTimeout <= 100 || options.ConnectTimeout >= 30000)
                return ValidateOptionsResult.Fail("Redis ConnectTimeout must be between 100 and 30000 milliseconds.");

            if (options.SyncTimeout == null)
                return ValidateOptionsResult.Fail("Redis SyncTimeout must be defined when Redis is enabled.");

            if (options.SyncTimeout <= 100 || options.SyncTimeout >= 30000)
                return ValidateOptionsResult.Fail("Redis SyncTimeout must be between 100 and 30000 milliseconds.");


            return ValidateOptionsResult.Success;
        }
    }
}
