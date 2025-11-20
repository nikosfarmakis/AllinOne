using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class AccessSection
    {
        public Dictionary<string, UserWithAccess> UsersWithAccess { get; set; } = new();
    }

    public class UserWithAccess
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Apikey { get; set; }
    }
}
