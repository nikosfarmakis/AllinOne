using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteDatabase
{
    public abstract class CommonFields
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        //public bool IsDeleted { get; set; } = false;

    }
}
