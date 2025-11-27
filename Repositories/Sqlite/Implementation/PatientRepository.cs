using AllinOne.Data.Sqlite;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Repositories.Sqlite.Interface;

namespace AllinOne.Repositories.Sqlite.Implementation
{
    public class PatientRepository : SqliteRepository<Patient>, IPatientRepository
    {
        public PatientRepository(DbContextSqlite context, ILogger<PatientRepository> logger) : base(context, logger)
        {

        }
    }
}
