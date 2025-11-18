using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Services.Implementations.PaginationStrategies;
using AllinOne.Services.Interfaces;
using AllinOne.Services.Interfaces.PaginationStrategies;
using AllinOne.Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AllinOne.Services.Implementations
{
    //new
    public class PaginationService<T> : IPaginationService<T> where T : class
    {
        private readonly ILogger<PaginationService<T>> _logger;
        private readonly IPaginationStrategyFactory<T> _factory;
        private readonly PaginationExpressionHelper<T> _exprHelper;

        public PaginationService(ILogger<PaginationService<T>> logger,
             IPaginationStrategyFactory<T> factory,
             PaginationExpressionHelper<T> exprHelper)
        {
            _logger = logger;
            _factory = factory;
            _exprHelper = exprHelper;
        }

        public async Task<PaginatedResponse<TOut>> ApplyPaginationAsync<TOut>(
            IQueryable<T> query, PaginationQuery pagination, string sortBy, bool sortAscending, Func<T, TOut> selector)
            where TOut : class
        {
            try
            {
                IQueryable<T> orderedQuery = _exprHelper.ApplyOrdering(query, sortBy, sortAscending);

                var totalRecords = await orderedQuery.CountAsync();
                var strategy = _factory.GetStrategy<TOut>(totalRecords);
                return await strategy.PaginateAsync(orderedQuery, pagination, sortBy, sortAscending, selector);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} <{EntityName}>", nameof(ApplyPaginationAsync), typeof(T).Name);
                throw;
            }
        }
    }
}
