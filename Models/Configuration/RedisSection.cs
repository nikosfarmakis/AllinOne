using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class RedisSection
    {
        public bool Enabled { get; set; }
        [Required]
        public string Configuration {  get; set; }
        [Required]
        public string InstanceName { get; set; }
        public bool Ssl { get; set; }
        public bool AbortOnConnectFail { get; set; }
        [Range(100, 30000)]
        public int ConnectTimeout { get; set; }
        [Range(100, 30000)]
        public int SyncTimeout { get; set; }

    }
}
