using AllinOne.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AllinOne.Data.Sqlite.Extensions
{
    public static class SqliteExtensions
    {
        public static IServiceCollection AddSqliteDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DbContextSqlite>(options =>
                options.UseSqlite(connectionString)
                       //.UseLazyLoadingProxies()
                       );

            return services;
        }
    }
}
