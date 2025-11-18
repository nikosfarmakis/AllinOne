using AllinOne.Models.Configuration;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Services.Interfaces.PaginationStrategies;
using AllinOne.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AllinOne.Services.Implementations.PaginationStrategies
{
    public class OffsetPaginationStrategy<T, TOut> : IPaginationStrategy<T, TOut>
        where T : class where TOut : class
    {
        private readonly ILogger<OffsetPaginationStrategy<T, TOut>> _logger;
        private readonly PaginationSection _options;
        private readonly IWebHostEnvironment _env;


        public OffsetPaginationStrategy(ILogger<OffsetPaginationStrategy<T, TOut>> logger,
            IOptions<PaginationSection> options,
            IWebHostEnvironment env)

        {
            _logger = logger;
            _options = options.Value;
            _env = env;
        }

        public async Task<PaginatedResponse<TOut>> PaginateAsync(
            IQueryable<T> query,
            PaginationQuery pagination,
            string sortBy,
            bool sortAscending,
            Func<T, TOut> selector)
        {
            try
            {
                var totalRecords = await query.CountAsync();
                var pageSize = Math.Clamp(pagination.PageSize ?? _options.DefaultPageSize, _options.MinPageSize, _options.MaxPageSize);
                var skip = ((int)pagination.Page - 1) * pageSize;

                //var items = await query.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();
                var items = await query.Skip(skip).Take(pageSize).AsNoTracking().LogAndExecuteAsync(_logger, _env, "Paginate Users");
                var projected = items.Select(selector);

                return new PaginatedResponse<TOut>(pagination.Page, pageSize, totalRecords, projected);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in {Strategy} in {Operation} for entity {EntityName}",
                    nameof(OffsetPaginationStrategy<T, TOut>), nameof(PaginateAsync), typeof(T).Name);
                throw;
            }
        }
    }
}