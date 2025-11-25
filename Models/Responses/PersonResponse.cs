using AllinOne.Models.SqliteDatabase;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.Responses
{
    public abstract class PersonResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; internal set; }
        public DateTime? DateOfBirth { get; internal set; }
    }
}
