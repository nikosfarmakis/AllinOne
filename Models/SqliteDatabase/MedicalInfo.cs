using AllinOne.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllinOne.Models.SqliteDatabase
{
    public class MedicalInfo : CommonFields
    {
        // 1-1 relationship
        public Guid PatientId { get; set; } // FK Patient
        public Patient Patient { get; set; } // navigation

        [NotMapped]
        public bool HasDrugAllergies => !DrugAllergiesDescription.IsNullOrEmptyOrWhitespace();
        [MaxLength(2000)]
        public string DrugAllergiesDescription { get; set; }
        [NotMapped]
        public bool HasGeneralAllergies => !GeneralAllergiesDescription.IsNullOrEmptyOrWhitespace();
        [MaxLength(2000)]
        public string GeneralAllergiesDescription { get; set; }
        [MaxLength(2000)]
        public string ChronicConditions { get; set; }
        [MaxLength(2000)]
        public string PastSurgeries { get; set; }
        [NotMapped]
        public bool Smokes => CigarettesPerDay>0;
        [Range(0, 200)] // 0 = non smoker
        public int CigarettesPerDay { get; set; } = 0;
        [NotMapped]
        public bool DrinksAlcohol => DrinksPerWeek > 0;
        [Range(0, 50)] // 0 = non drink
        public int DrinksPerWeek { get; set; } = 0;
        [MaxLength(3000)]
        public string AdditionalNotes { get; set; }
    }
}
