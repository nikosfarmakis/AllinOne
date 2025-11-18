using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class JwtSettings
    {
        [Required]
        public string Secret { get; set; } = string.Empty;
        [Required]
        public string Issuer { get; set; } = string.Empty;
        [Required]
        public string Audience { get; set; } = string.Empty;
        [Range(1, 120)]
        public int ExpirationMinutes { get; set; } = 30;  // default
        [Range(60, 1800)]
        public int SlidingExpirationSeconds { get; set; } = 600; // 10min refresh window
        [MinLength(1)] // for [HttpPost("login")]
        public List<string> WhiteListEndPoints { get; set; } = new();
    }
}
