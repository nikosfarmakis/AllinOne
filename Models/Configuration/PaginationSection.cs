using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class PaginationSection
    {
        [Range(1, 1000)]
        public int CursorΒasedAfterRecords {  get; set; }
        [Range(1, 100)]
        public int DefaultPageSize {  get; set; }
        [Range(1, 100)]
        public int MaxPageSize {  get; set; }
        [Range(1, 100)]
        public int MinPageSize { get; set; }
        public string DefaultSortField { get; set; }
        public bool DefaultSortDirectionAsc { get; set; }
    }
}
