using AllinOne.Utils.Extensions;
using System.ComponentModel.DataAnnotations;

public static class UserValidators
{
    public static ValidationResult ValidateEmail(string email, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new ValidationResult("Email cannot be empty.");

        if (!email.IsValidEmail(out List<string> errors))
            return new ValidationResult(string.Join("; ", errors));

        return ValidationResult.Success;
    }

    public static ValidationResult ValidatePhone(string phone, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return new ValidationResult("Phone cannot be empty.");

        if (!phone.IsValidPhone(out string error))
            return new ValidationResult(error);

        return ValidationResult.Success;
    }
}
