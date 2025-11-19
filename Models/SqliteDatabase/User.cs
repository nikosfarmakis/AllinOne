using AllinOne.Constants;
using Microsoft.EntityFrameworkCore;

namespace AllinOne.Models.SqliteDatabase
{
    [Index(nameof(LastName), nameof(FirstName), IsUnique = false)]

    public class User: Person
    {
        public UserRoles Role { get; internal set; }
        public bool IsAdmin { get; internal set; }
    }
}
