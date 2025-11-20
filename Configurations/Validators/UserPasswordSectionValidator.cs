using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class UserPasswordSectionValidator : IValidateOptions<UserPasswordSection>
    {
        public ValidateOptionsResult Validate(string name, UserPasswordSection options)
        {
            if (options.MinLength >= options.MaxLength)
                return ValidateOptionsResult.Fail("UserPasswordSettings MinLength must be less than MaxLength.");

            int diference = 5; 
            if (options.MaxLength - options.MinLength < diference)
                return ValidateOptionsResult.Fail($"UserPasswordSettings MaxLength must be at least {diference} characters greater than MinLength.");

            return ValidateOptionsResult.Success;
        }
    }
}
