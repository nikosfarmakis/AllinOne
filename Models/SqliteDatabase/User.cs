using AllinOne.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(LastName), nameof(FirstName), IsUnique = false)]

    public class User: Person
    {
        [Required]
        public UserRoles Role { get; internal set; }
        [NotMapped]
        public bool IsAdmin => Role == UserRoles.Admin;
        [Required]
        public string Password {  get; internal set; }

    }
}
