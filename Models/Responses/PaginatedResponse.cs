using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AllinOne.Models.Responses
{
    public class PaginatedResponse<T> where T : class
    {
        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int? PageSize { get; set; }

        [JsonPropertyName("recordsTotal")]
        public int? RecordsTotal { get; set; }

        [JsonPropertyName("pagesTotal")]
        public int? PagesTotal => (RecordsTotal.HasValue && PageSize.HasValue)
                                    ? (int)Math.Ceiling((double)RecordsTotal.Value / PageSize.Value)
                                    : null;
        [JsonPropertyName("nextCursor")]
        public string? NextCursor { get; set; }

        [JsonPropertyName("hasNextPage")]
        public bool? HasNextPage => Page.HasValue && RecordsTotal.HasValue && PageSize.HasValue ? RecordsTotal > (Page * PageSize) : null;
        [JsonPropertyName("lastPage")]
        public bool? LastPage => Page.HasValue && HasNextPage.HasValue ? !HasNextPage.Value : null;
        [JsonPropertyName("isOffsetBased")]
        public bool IsOffsetBased => Page.HasValue;

        public PaginatedResponse(int page, int pageSize,int recordsTotal, IEnumerable<T> items)
        {
            Page = page;
            PageSize = pageSize;
            RecordsTotal = recordsTotal;
            Items = items;
        }

        public PaginatedResponse(string? nextCursor, int pageSize, int recordsTotal, IEnumerable<T> items)
        {
            NextCursor = nextCursor;
            Items = items;
            PageSize = pageSize;
            RecordsTotal = recordsTotal;

        }

    }
}
