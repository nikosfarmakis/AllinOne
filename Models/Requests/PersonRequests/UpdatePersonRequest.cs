using AllinOne.Models.SqliteDatabase;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Requests.PersonRequests
{
    public abstract class UpdatePersonRequest
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(13)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? Email { get; internal set; }
        public DateTime? DateOfBirth { get; internal set; }
    }
}
