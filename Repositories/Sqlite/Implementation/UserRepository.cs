using AllinOne.Data.Sqlite;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Repositories.Sqlite.Interface;

namespace AllinOne.Repositories.Sqlite.Implementation
{
    public class UserRepository : SqliteRepository<User>, IUserRepository
    {
        public UserRepository(DbContextSqlite context, ILogger<UserRepository> logger) : base(context, logger)
        {

        }
    }
}
