using Newtonsoft.Json;

namespace AllinOne.Models.Responses
{
    public class OrderResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
