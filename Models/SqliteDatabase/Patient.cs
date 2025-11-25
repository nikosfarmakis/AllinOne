using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(AMKA), IsUnique = true)]
    [Index(nameof(LastName), nameof(FirstName), IsUnique = false)]

    public class Patient : Person
    {        
        // 1-1 relationship
        public MedicalInfo? MedicalInfo { get; set; }

        [Required]
        public string AMKA { get; set; }
        public string? Notes { get; set; }

    }
}