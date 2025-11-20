using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Validators
{
    public class PaginationSectionValidator : IValidateOptions<PaginationSection>
    {
        public ValidateOptionsResult Validate(string name, PaginationSection options)
        {
            if (options == null)
                return ValidateOptionsResult.Fail("PaginationSettings cannot be null.");

            if (string.IsNullOrWhiteSpace(options.DefaultSortField))
                return ValidateOptionsResult.Fail("DefaultSortField cannot be empty.");

            if (!(options.MinPageSize < options.DefaultPageSize
               && options.DefaultPageSize < options.MaxPageSize))
            {
                return ValidateOptionsResult.Fail("MinPageSize < DefaultPageSize < MaxPageSize must be true.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
