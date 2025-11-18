using Newtonsoft.Json;

namespace AllinOne.Models.Requests
{
    public class UpdateOrderRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
