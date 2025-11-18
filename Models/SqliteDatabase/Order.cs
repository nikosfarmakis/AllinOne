
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteEntities
{
    [Index(nameof(Description), IsUnique = false)]
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        //[Required]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //ICollection ONLY list for Explicit Loading
    }
}
