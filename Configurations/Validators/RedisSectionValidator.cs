using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class RedisSectionValidator : IValidateOptions<RedisSection>
    {
        public ValidateOptionsResult Validate(string name, RedisSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("RedisSettings cannot be null.");

            if (options.Enabled)
            {
                /*if (string.IsNullOrWhiteSpace(options.Configuration))
                    return ValidateOptionsResult.Fail("Redis configuration string cannot be empty when Redis is enabled.");

                if (string.IsNullOrWhiteSpace(options.InstanceName))
                    return ValidateOptionsResult.Fail("Redis instance name cannot be empty when Redis is enabled.");

                if (options.ConnectTimeout <= 0)
                    return ValidateOptionsResult.Fail("ConnectTimeout must be greater than zero.");

                if (options.SyncTimeout <= 0)
                    return ValidateOptionsResult.Fail("SyncTimeout must be greater than zero.");*/
            }

            return ValidateOptionsResult.Success;
        }
    }
}
