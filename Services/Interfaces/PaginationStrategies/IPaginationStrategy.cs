using AllinOne.MemoryCache.Interfaces;
using AllinOne.Models.Configuration;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;

namespace AllinOne.Services.Interfaces.PaginationStrategies
{
    public interface IPaginationStrategy<T, TOut> where T : class where TOut : class
    {
        Task<PaginatedResponse<TOut>> PaginateAsync(
            IQueryable<T> query,
            PaginationQuery pagination,
            string sortBy,
            bool sortAscending,
            Func<T, TOut> selector);
    }
}
