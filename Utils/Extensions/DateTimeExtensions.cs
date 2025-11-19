namespace AllinOne.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool ValidDateOfBirth(this DateTime dob, out string error)
        {
            error = string.Empty;

            if (dob > DateTime.Today)
            {
                error = "Date of birth cannot be in the future.";
                return false;
            }

            int age = (int)((DateTime.Today - dob).TotalDays / 365.25);
            if (age > 130)
            {
                error = "Age cannot exceed 130 years.";
                return false;
            }

            return true;
        }
    }
}
