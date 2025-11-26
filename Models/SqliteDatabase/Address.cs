using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AllinOne.Models.SqliteDatabase
{
    public class Address :CommonFields
    {
        // 1-1 relationship
        public Guid PersonId { get; set; }   // FK Person
        public Person Person { get; set; }   // navigation

        [MaxLength(200)]
        public string? Street { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Country { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PostalCode { get; set; } = string.Empty;

    }
}

