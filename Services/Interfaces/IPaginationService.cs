using AllinOne.Models.Requests;
using AllinOne.Models.Responses;

namespace AllinOne.Services.Interfaces
{
    public interface IPaginationService<T> where T : class
    {
        Task<PaginatedResponse<TOut>> ApplyPaginationAsync<TOut>(IQueryable<T> query, PaginationQuery pagination, string sortBy, bool sortAscending,
                                      Func<T, TOut> selector) where TOut : class;
    }
}