using Newtonsoft.Json;

namespace AllinOne.Models.Requests
{
    public class CreateOrderRequest
    {
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
