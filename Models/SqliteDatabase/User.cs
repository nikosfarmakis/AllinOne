using AllinOne.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(LastName), nameof(FirstName), IsUnique = false)]

    public class User: Person
    {
        [Required]
        public UserRoles Role { get; internal set; }
        public bool IsAdmin { get; internal set; }
        [Required]
        public string Password {  get; internal set; }

    }
}
