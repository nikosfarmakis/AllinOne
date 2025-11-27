using Newtonsoft.Json;

namespace AllinOne.Models.Requests.OrdrRequests
{
    public class UpdateOrderRequest : IUpdateRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
