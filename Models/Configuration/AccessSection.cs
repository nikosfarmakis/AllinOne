using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class AccessSection
    {
        public Dictionary<string, UserWithAccess> UsersWithAccess { get; set; } = new();
    }

    public class UserWithAccess
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Apikey { get; set; }
    }
}
