using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class UserPasswordSection
    {
        [Range(1, 50)]
        public int MinLength { get; set; } = 8;
        [Range(1, 50)]
        public int MaxLength { get; set; } = 15;
        public bool RequireLettersChar { get; set; } = true;
        public bool RequireNumbers { get; set; } = true;
        public bool RequireSpecialChar { get; set; } = false;

    }
}
