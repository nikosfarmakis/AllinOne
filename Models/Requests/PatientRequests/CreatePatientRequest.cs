using AllinOne.Models.Requests.PersonRequests;
using AllinOne.Models.SqliteDatabase.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Requests.PatientRequests
{
    public class CreatePatientRequest : CreatePersonRequest
    {
        public string? Notes { get; set; }
        [Required]
        public string AMKA { get; set; }
        public PatientMedicalInfo? PatientMedicalInfo { get; set; }
    }
}
