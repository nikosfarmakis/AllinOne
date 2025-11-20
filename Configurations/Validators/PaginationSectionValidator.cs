using AllinOne.Models.Configuration;
using AllinOne.Utils.Extensions;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class PaginationSectionValidator : IValidateOptions<PaginationSection>
    {
        public ValidateOptionsResult Validate(string name, PaginationSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("PaginationSettings cannot be null.");

            if (options.DefaultSortField.IsNullOrEmptyOrWhitespace())
                return ValidateOptionsResult.Fail("DefaultSortField cannot be empty.");

            if (!(options.MinPageSize < options.DefaultPageSize
               && options.DefaultPageSize < options.MaxPageSize))
            {
                return ValidateOptionsResult.Fail("MinPageSize must be less than DefaultPageSize, and DefaultPageSize must be less than MaxPageSize.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
