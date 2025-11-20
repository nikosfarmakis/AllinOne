using AllinOne.Models.Configuration;
using AllinOne.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class JwtSectionValidator : IValidateOptions<JwtSection>
    {
        public ValidateOptionsResult Validate(string name, JwtSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("JwtSettings configuration is missing.");

            if (options.Secret.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("JWT Secret cannot be empty.");

            if (options.Issuer.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("JWT Issuer cannot be empty.");

            if (options.Audience.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("JWT Audience cannot be empty.");

            if (!options.WhiteListEndPoints.Any())
                return ValidateOptionsResult.Fail("At least one endpoint must be defined in WhiteListEndPoints.");

            for (int i = 0; i < options.WhiteListEndPoints.Count; i++)
            {
                if (options.WhiteListEndPoints[i].IsNullOrEmptyOrWhitespace())
                    return ValidateOptionsResult.Fail($"WhiteListEndPoints[{i}] contains an invalid (empty) endpoint.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
