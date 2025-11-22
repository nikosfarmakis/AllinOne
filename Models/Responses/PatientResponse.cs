using AllinOne.Models.SqliteDatabase.ValueObjects;

namespace AllinOne.Models.Responses
{
    public class PatientResponse : PersonResponse
    {
        public string? Notes { get; set; }
        public string AMKA { get; set; }
        public PatientMedicalInfo? PatientMedicalInfo { get; set; }
    }
}
