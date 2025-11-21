using AllinOne.Models.Requests.PersonRequests;
using AllinOne.Models.SqliteDatabase.ValueObjects;

namespace AllinOne.Models.Requests.PatientRequests
{
    public class UpdatePatientRequest : UpdatePersonRequest
    {
        public string Notes { get; set; }
        public string AMKA { get; set; }
        public PatientMedicalInfo? PatientMedicalInfo { get; set; }
    }
}
