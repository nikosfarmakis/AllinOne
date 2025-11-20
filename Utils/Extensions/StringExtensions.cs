using System.Text.RegularExpressions;

namespace AllinOne.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string EncodeToBase64(this string dataSTR)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataSTR));
        }

        public static string DecodeFromBase64(this string dataSTR)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(dataSTR));
        }

        private const int MaxEmailLength = 254;
        private const int MaxLocalLength = 64;
        private static readonly string AtomChars = "!#$%&'*+/=?^_`{|}~-";

        public static bool IsValidEmail(this string email, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
            {
                validationErrors.Add("Email is null or empty.");
                return false;
            }

            email = email.Trim();

            if (email.EndsWith("."))
            {
                validationErrors.Add("Email ends with a dot.");
            }

            int atIndex = email.LastIndexOf('@');
            if (atIndex < 1 || atIndex == email.Length - 1)
            {
                validationErrors.Add("Email must contain a single '@' with local and domain parts.");
            }

            if (validationErrors.Count > 0)
            {
                return false;
            }

            string localPart = email.Substring(0, atIndex);
            string domainPart = email.Substring(atIndex + 1);

            if (domainPart.Length == 0 || domainPart.Contains(" "))
            {
                validationErrors.Add("Domain is empty or contains spaces.");
            }

            string normalizedEmail = $"{localPart}@{domainPart}";

            if (normalizedEmail.Length > MaxEmailLength)
            {
                validationErrors.Add($"Email exceeds maximum total length of {MaxEmailLength}.");
            }

            if (localPart.Length > MaxLocalLength)
            {
                validationErrors.Add($"Local part exceeds maximum length of {MaxLocalLength}.");
            }

            if (!IsValidLocalPart(localPart, out List<string> localErrors))
            {
                validationErrors.AddRange(localErrors);
            }

            if (!IsValidDomain(domainPart, out List<string> domainErrors))
            {
                validationErrors.AddRange(domainErrors);
            }

            return validationErrors.Count == 0;
        }

        private static bool IsValidLocalPart(string local, out List<string> errors)
        {
            errors = new List<string>();

            if (local.StartsWith("\"") && local.EndsWith("\""))
            {
                for (int i = 1; i < local.Length - 1; i++)
                {
                    char c = local[i];
                    if (c <= 31 || c == 127)
                        errors.Add("Quoted local part contains control characters.");
                }
                return errors.Count == 0;
            }

            string[] atoms = local.Split('.');
            if (atoms.Length == 0)
            {
                errors.Add("Local part is empty.");
                return false;
            }

            foreach (var atom in atoms)
            {
                if (string.IsNullOrEmpty(atom))
                {
                    errors.Add("Local part has empty atom (double dots or leading/trailing dot).");
                    continue;
                }

                foreach (char c in atom)
                {
                    if (!char.IsLetterOrDigit(c) && !AtomChars.Contains(c))
                        errors.Add($"Invalid character '{c}' in local part.");
                }
            }

            return errors.Count == 0;
        }

        private static bool IsValidDomain(string domain, out List<string> errors)
        {
            errors = new List<string>();

            string[] labels = domain.Split('.');
            if (labels.Length < 2)
            {
                errors.Add("Domain must have at least two labels.");
                return false;
            }

            foreach (var label in labels)
            {
                if (label.Length == 0)
                {
                    errors.Add("Domain contains empty label (double dots).");
                    continue;
                }

                if (label.Length > 63)
                    errors.Add($"Domain label '{label}' exceeds maximum length of 63.");

                if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[^1]))
                    errors.Add($"Domain label '{label}' must start and end with a letter or digit.");

                foreach (char c in label)
                {
                    if (!(char.IsLetterOrDigit(c) || c == '-'))
                        errors.Add($"Invalid character '{c}' in domain label '{label}'.");
                }
            }

            return errors.Count == 0;
        }

        public static bool IsValidateName(this string name, string field, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                error = $"{field} cannot be empty or whitespace.";
                return false;
            }

            if (name.Length < 2 || name.Length > 30)
            {
                error = $"{field} must be between 2 and 30 characters.";
                return false;
            }

            if (!Regex.IsMatch(name, @"^[A-Za-zΑ-Ωα-ωΆ-Ώά-ώ]+$"))
            {
                error = $"{field} must contain only letters.";
                return false;
            }
            return true;
        }

        public static bool IsValidPhone(this string phone, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(phone))
            {
                error = "Phone number cannot be empty.";
                return false;
            }

            string cleaned = Regex.Replace(phone, @"[\s\-\(\)\.]", "");

            if (cleaned.StartsWith("+"))
            {
                string digitsOnly = cleaned.Substring(1);

                // After the "+" MUST be digits only
                if (!Regex.IsMatch(digitsOnly, @"^\d+$"))
                {
                    error = "Phone number after '+' must contain digits only.";
                    return false;
                }

                if (cleaned.Length != 13)
                {
                    error = "Phone number with '+' must be exactly 13 characters long.";
                    return false;
                }

                return true;
            }

            // Case 2: No "+" → must be digits only
            if (!Regex.IsMatch(cleaned, @"^\d+$"))
            {
                error = "Phone number must contain digits only.";
                return false;
            }

            // Must be exactly 10 digits if no +
            if (cleaned.Length != 10)
            {
                error = "Phone number must be exactly 10 digits long.";
                return false;
            }

            return true;
        }

        public static bool IsValidAMKA(this string amka, out string error, DateTime? dateOfBirth = null)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(amka))
            {
                error = "AMKA cannot be empty.";
                return false;
            }

            if (amka.Length != 11)
            {
                error = "AMKA must contain exactly 11 digits.";
                return false;
            }

            if (!amka.All(char.IsDigit))
            {
                error = "AMKA must contain only numeric characters.";
                return false;
            }

            // If date of birth is supplied, validate consistency
            if (dateOfBirth != null)
            {
                string dobPart = dateOfBirth.Value.ToString("ddMMyy");

                if (!amka.StartsWith(dobPart))
                {
                    error = $"AMKA does not match the provided date of birth. Expected prefix: {dobPart}.";
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidPassword(this string pass,out string error,int minLength, int maxLength,
                    bool requireLettersChar = true,bool requireNumbers = true, bool requireSpecialChar = true)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(pass))
            {
                error = "Password cannot be empty.";
                return false;
            }

            if (pass.Length < minLength)
            {
                error = $"Password must be at least {minLength} characters long.";
                return false;
            }

            if (pass.Length > maxLength)
            {
                error = $"Password cannot exceed {maxLength} characters.";
                return false;
            }

            if (requireLettersChar && !pass.Any(char.IsLetter))
            {
                error = "Password must contain at least one letter.";
                return false;
            }

            if (requireNumbers && !pass.Any(char.IsDigit))
            {
                error = "Password must contain at least one number.";
                return false;
            }

            if (requireSpecialChar && pass.All(char.IsLetterOrDigit))
            {
                error = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }
    }
}
