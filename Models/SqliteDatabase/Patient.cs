using AllinOne.Models.SqliteDatabase.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(AMKA), IsUnique = true)]
    [Index(nameof(LastName), nameof(FirstName), IsUnique = false)]

    public class Patient : Person
    {
        public string? Notes { get; set; }
        [Required]
        public string AMKA { get; set; }
        public PatientMedicalInfo? PatientMedicalInfo { get; set; }
    }
}