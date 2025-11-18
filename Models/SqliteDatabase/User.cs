using AllinOne.Constants;
using AllinOne.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(LastName),nameof(FirstName), IsUnique = false)]
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [MaxLength(100)]
        [Required]
        public string FirstName { get; internal set; }
        [MaxLength(100)]
        [Required]
        public string LastName { get; internal set; }
        [Range(18, 120)]
        [Required]
        public int Age { get; internal set; }
        [Required]
        [MaxLength(200)]
        //[EmailAddress]// only MVC Forms, Blazor, Razor
        public string Email { get; internal set; }
        [MaxLength(13)]
        [Phone]
        public string Phone { get; internal set; }
        [Required]
        public Address HomeAddress { get; set; } = new();
        public UserRoles Role { get; internal set; }
        public bool IsAdmin { get; internal set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
        //calculated at runtime
        [NotMapped] 
        //It is at the base
        //It is automatically calculated by the database every time a SELECT or UPDATE
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public string DisplayName => $"{FirstName} {LastName}";

    }
}
