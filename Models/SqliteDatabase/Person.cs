using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllinOne.Models.SqliteDatabase
{
    public abstract class Person : CommonFields
    {
        // 1-1 relationship
        public Address? Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(13)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [NotMapped]
        public int? Age
        {
            get
            {
                if (DateOfBirth is null)
                    return null;

                var dob = DateOfBirth.Value;
                var today = DateTime.UtcNow;

                var age = today.Year - dob.Year;

                if (dob.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }    
        [NotMapped] //calculated at runtime by C#
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] //It is at the base //It is automatically calculated by the database every time a SELECT or UPDATE
        public string DisplayName => $"{FirstName} {LastName}";
    }
}
