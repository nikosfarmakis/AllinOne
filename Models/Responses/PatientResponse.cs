using AllinOne.Models.SqliteDatabase;

namespace AllinOne.Models.Responses
{
    public class PatientResponse : PersonResponse
    {
        public string? Notes { get; set; }
        public string AMKA { get; set; }
    }
}
