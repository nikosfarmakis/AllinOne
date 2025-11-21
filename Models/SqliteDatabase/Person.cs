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
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Email { get; internal set; }
        public DateTime DateOfBirth { get; internal set; }
        public Address? HomeAddress { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.UtcNow;
                var age = today.Year - DateOfBirth.Year;

                if (DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
        //calculated at runtime by C#
        [NotMapped]
        //It is at the base
        //It is automatically calculated by the database every time a SELECT or UPDATE
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public string DisplayName => $"{FirstName} {LastName}";
    }

}
