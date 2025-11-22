using AllinOne.Models.SqliteDatabase.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllinOne.Models.SqliteDatabase
{
    public abstract class Person : CommonFields
    {
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
        public Address? HomeAddress { get; set; }
        public bool IsDeleted { get; set; } = false;
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
