using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Configuration
{
    public class PaginationSection
    {
        [Required]
        public int CursorΒasedAfterRecords {  get; set; }
        [Required]
        public int DefaultPageSize {  get; set; }
        [Required]
        public int MaxPageSize {  get; set; }
        [Required]
        public int MinPageSize { get; set; }
        [Required]
        public string DefaultSortField { get; set; }
        [Required]
        public bool DefaultSortDirectionAsc { get; set; }
    }
}
