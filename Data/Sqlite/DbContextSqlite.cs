using AllinOne.Models.SqliteEntities;
using Microsoft.EntityFrameworkCore;

namespace AllinOne.Data.Sqlite
{
    public class DbContextSqlite : DbContext
    {
        public DbContextSqlite(DbContextOptions<DbContextSqlite> options) : base(options) 
        { 

        }

        public DbSet<Order> Orders { get; set; }
    }
}
