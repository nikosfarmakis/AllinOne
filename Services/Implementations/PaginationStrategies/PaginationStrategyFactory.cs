using AllinOne.Models.Configuration;
using AllinOne.Services.Interfaces.PaginationStrategies;
using Microsoft.Extensions.Options;
using AllinOne.Constants;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Services.Implementations.PaginationStrategies
{
    public class PaginationStrategyFactory<T> : IPaginationStrategyFactory<T> where T : class
    {
        private readonly PaginationSection _options;
        private readonly ILogger<PaginationStrategyFactory<T>> _logger;
        private readonly IKeyedServiceProvider _keyedProvider;//{4}
        //private readonly IServiceProvider _serviceProvider; //{1}
        /*        private readonly OffsetPaginationStrategy<T, object> _offset; //{2}
                private readonly CursorPaginationStrategy<T, object> _cursor; //{2}*/
        //private readonly IEnumerable<IPaginationStrategy<T, object>> _strategies; //{3}

        public PaginationStrategyFactory(//IServiceProvider serviceProvider,//{1}
            ILogger<PaginationStrategyFactory<T>> logger,
            IOptions<PaginationSection> options,
            IKeyedServiceProvider keyedProvider//{4}
            //IEnumerable<IPaginationStrategy<T, object>> strategies //{3}
            //OffsetPaginationStrategy<T, object> offset,//{2}
            //CursorPaginationStrategy<T, object> cursor//{2}
            )
        {
             _logger = logger;
            _options = options.Value;
            _keyedProvider = keyedProvider;//{4}
            //_serviceProvider = serviceProvider;//{1}
            //_strategies = strategies; //{3}
/*            _offset = offset;//{2}
            _cursor = cursor;//{2}*/
        }

        public IPaginationStrategy<T, TOut> GetStrategy<TOut>(int totalRecords) where TOut : class
        {
            try
            {
                //------------------------------- 4
                var key = totalRecords <= _options.CursorΒasedAfterRecords ?
                    PaginationStrategiesCategs.Offset.GetDisplayName() : PaginationStrategiesCategs.Cursor.GetDisplayName();
                return _keyedProvider.GetRequiredKeyedService<IPaginationStrategy<T, TOut>>(key);
                //------------------------------- 1
                /*  bool useOffset = totalRecords <= _options.CursorΒasedAfterRecords;
                    return useOffset//{1} return an instance of the concrete type
                        ? (IPaginationStrategy< T,TOut > )_serviceProvider.GetRequiredService<OffsetPaginationStrategy<T, TOut>>()
                        : (IPaginationStrategy<T, TOut>)_serviceProvider.GetRequiredService<CursorPaginationStrategy<T, TOut>>();*/

                //------------------------------- 3
/*                bool useOffset = totalRecords <= _options.CursorΒasedAfterRecords;
                var strategy = useOffset //{3}
                                ? _strategies.FirstOrDefault(s => s is OffsetPaginationStrategy<T, object>)
                                : _strategies.FirstOrDefault(s => s is CursorPaginationStrategy<T, object>);
                if (strategy is null)
                {
                    throw new InvalidOperationException("No suitable pagination strategy registered.");
                }
                return (IPaginationStrategy<T, TOut>)strategy;*/
                //------------------------------ 2

                //return useOffset ? (IPaginationStrategy<T, TOut>)_offset : (IPaginationStrategy<T, TOut>)_cursor; //{2}


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} <{EntityName}>", nameof(GetStrategy), typeof(T).Name);
                throw ex;
            }
        }
    }
}
