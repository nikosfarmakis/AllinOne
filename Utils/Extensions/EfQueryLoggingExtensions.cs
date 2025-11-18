using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AllinOne.Utils.Extensions
{
    public static class EfQueryLoggingExtensions
    {
        public static async Task<List<T>> LogAndExecuteAsync<T>(
            this IQueryable<T> query,
            ILogger logger,
            IWebHostEnvironment env,
            string? description = null) where T : class
        {
            var sql = query.ToQueryString();
            var sw = Stopwatch.StartNew();

            var result = await query.ToListAsync();

            sw.Stop();

            // Development Mode (Full Debug Info)
            if (env.IsDevelopment())
            {
                logger.LogDebug(
                    """
                        ====== EF QUERY DEBUG ======
                        Description: {Description}
                        Entity: {Entity}
                        Rows: {Count}
                        Time: {Elapsed} ms

                        SQL:
                        {Sql}

                        ============================
                        """,
                    description ?? typeof(T).Name,
                    typeof(T).Name,
                    result.Count,
                    sw.ElapsedMilliseconds,
                    sql
                );
            }
            // Production Mode (Safe Logging)
            else
            {
                logger.LogInformation(
                    """
                        [EF Query] {Description}
                        Time: {Elapsed} ms
                        Rows: {Count}
                        SQL (safe):
                        {Sql}
                        """,
                    description ?? typeof(T).Name,
                    sw.ElapsedMilliseconds,
                    result.Count,
                    sql
                );
            }

            return result;
        }
    }

}
