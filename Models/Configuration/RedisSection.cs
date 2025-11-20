using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class RedisSection
    {
        public bool Enabled { get; set; }
        public string Configuration {  get; set; }
        public string InstanceName { get; set; }
        public bool Ssl { get; set; }
        public bool? AbortOnConnectFail { get; set; }
        public int? ConnectTimeout { get; set; }
        public int? SyncTimeout { get; set; }

    }
}
