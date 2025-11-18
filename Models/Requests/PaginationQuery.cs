using Newtonsoft.Json;

namespace AllinOne.Models.Requests
{
    public class PaginationQuery
    {
        // Offset-based
        [JsonProperty("page")]
        public int Page { get; set; } = 1;

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        // Cursor-based
        [JsonProperty("after")]
        public string? After { get; set; }

    }
}
