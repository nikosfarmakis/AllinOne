
using AllinOne.Models.SqliteDatabase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteEntities
{
    [Index(nameof(Description), IsUnique = false)]
    public class Order : CommonFields
    {
        [MaxLength(200)]
        public string? Description { get; set; }

        //ICollection ONLY list for Explicit Loading
    }
}
