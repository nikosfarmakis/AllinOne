using AllinOne.Models.Configuration;
using AllinOne.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class AccessSectionValidator : IValidateOptions<AccessSection>
    {
        public ValidateOptionsResult Validate(string? name, AccessSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("AccessSettings configuration is missing.");

            if (options.UsersWithAccess == null || !options.UsersWithAccess.Any())
            {
                return ValidateOptionsResult.Fail("At least one validation user must be defined in UsersWithAccess.");
            }

            foreach (var (key, user) in options.UsersWithAccess)
            {
                if (user.Username.IsNullOrEmptyOrWhitespace())
                    return ValidateOptionsResult.Fail($"Validation user '{key}' is missing Username.");
                if (user.Password.IsNullOrEmptyOrWhitespace())
                    return ValidateOptionsResult.Fail($"Validation user '{key}' is missing Password.");
                if (user.Apikey.IsNullOrEmptyOrWhitespace())
                    return ValidateOptionsResult.Fail($"Validation user '{key}' is missing Apikey.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
