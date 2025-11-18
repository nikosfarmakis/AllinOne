using AllinOne.Models.SqliteEntities;
using AllinOne.Repositories.Sqlite.Interface;
using Newtonsoft.Json;
using System.Web.Http.Filters;

namespace AllinOne.Models.Requests
{
    public sealed class OrderFilterQuery : IFilter<Order>
    {
        [JsonProperty("descriptionContains")]
        public string? DescriptionContains { get; set; }
        [JsonProperty("fromCreatedAt ")]
        public DateTime? FromCreatedAt { get; set; }
        [JsonProperty("toCreatedAt")]
        public DateTime? ToCreatedAt { get; set; }

        // Sorting
        [JsonProperty("sortBy")]
        public OrderSortOptions SortBy { get; set; } = OrderSortOptions.CreatedAt;

        [JsonProperty("sortAscending")]
        public bool SortAscending { get; set; } = true;

        public IQueryable<Order> Apply(IQueryable<Order> source)
        {
            if (!string.IsNullOrWhiteSpace(DescriptionContains))
            {
                source = source.Where(o => o.Description != null && o.Description.Contains(DescriptionContains));
            }

            if (FromCreatedAt.HasValue)
            {
                source = source.Where(o => o.CreatedAt >= FromCreatedAt.Value);
            }

            if (ToCreatedAt.HasValue)
            {
                source = source.Where(o => o.CreatedAt <= ToCreatedAt.Value);
            }

            return source;
        }
    }

    public enum OrderSortOptions
    {
        CreatedAt,
        Description,
        UpdatedAt
    }
}
