using Newtonsoft.Json;

namespace AllinOne.Models.Requests.OrdrRequests
{
    public class CreateOrderRequest
    {
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
