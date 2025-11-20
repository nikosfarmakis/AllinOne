using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class JwtSectionValidator : IValidateOptions<JwtSection>
    {
        public ValidateOptionsResult Validate(string name, JwtSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("JwtSettings configuration is missing.");

            if (options.WhiteListEndPoints == null || !options.WhiteListEndPoints.Any())
                return ValidateOptionsResult.Fail("At least one validation EndPoint must be defined in WhiteListEndPoints.");
            

            foreach (var endPoint in options.WhiteListEndPoints)
            {
                if (string.IsNullOrWhiteSpace(endPoint))
                    return ValidateOptionsResult.Fail($"Validation faild, EndPoint '{endPoint}' is IsNullOrWhiteSpace or Empty EndPoint.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
