using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class UserPasswordSectionValidator : IValidateOptions<UserPasswordSection>
    {
        public ValidateOptionsResult Validate(string name, UserPasswordSection options)
        {
            if (options.MinLength >= options.MaxLength)
                return ValidateOptionsResult.Fail("MinLength must be less than MaxLength.");

            return ValidateOptionsResult.Success;
        }
    }
}
