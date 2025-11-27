using AllinOne.Models.Requests;
using AllinOne.Models.Requests.OrdrRequests;
using AllinOne.Models.Responses;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using AllinOne.Services.Interfaces.ModelHandlingServices;
using AllinOne.Utils.Mappers.Interfaces;
using Microsoft.OpenApi.Extensions;
using Order = AllinOne.Models.SqliteEntities.Order;

namespace AllinOne.Services.Implementations.ModelHandlingServices
{
    public class OrderHandlingService : IOrderHandlingService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaginationService<Order> _paginationService;
        private readonly IEntityHandlingService<Order, OrderResponse> _entityService;
        private readonly ILogger<OrderHandlingService> _logger;
        private readonly IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> _orderMapper;

        public OrderHandlingService(IOrderRepository orderRepository,
            IEntityHandlingService<Order,OrderResponse> entityService,
            IPaginationService<Order> paginationService,
            ILogger<OrderHandlingService> logger,
            IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> orderMapper)
        {
            _orderRepository = orderRepository;
            _paginationService = paginationService;
            _entityService = entityService;
            _orderMapper = orderMapper;
            _logger = logger;
        }

        public  Task<ApiResult<OrderResponse>> CreateModelAsync(CreateOrderRequest request)
        {
            var entity = _orderMapper.ToEntity(request);
            //var entity = request.ToOrderDbModel();
            return  _entityService.CreateAsync(entity, o => _orderMapper.ToResponse(o));
        }

        public  Task<ApiResult<OrderResponse>> GetModelByIdAsync(Guid id)
        {
            return  _entityService.GetByIdAsync(id, o => _orderMapper.ToResponse(o));
        }

        public Task<ApiResult<bool>> DeleteModelByIdAsync(Guid id)
        {
            return _entityService.DeleteByIdAsync(id);
        }

        public Task<ApiResult<int>> DeleteAllAsync()
        {
            return _entityService.DeleteAllAsync();
        }

        public Task<ApiResult<OrderResponse>> UpdateModelAsync(UpdateOrderRequest request)
        {
            //entity -> Order from UpdateToOrder
            //public static void UpdateToOrder(this UpdateOrderRequest request, Order entity)]
            return _entityService.UpdateAsync(request.Id, entity => _orderMapper.UpdateEntity(request,entity), o => _orderMapper.ToResponse(o));
        }
        public async Task<ApiResult<PaginatedResponse<OrderResponse>>> GetPagedOrdersAsync(OrderFilterQuery filters, PaginationQuery pagination)
        {
            try
            {
                IQueryable<Order> query = _orderRepository.Query();
                if (filters != null)
                {
                    query = filters.Apply(query);
                }

                //Use pagination service to paginate & project to OrderResponse
                var paged = await _paginationService.ApplyPaginationAsync(query, pagination, filters.SortBy.GetDisplayName(), filters.SortAscending, o => _orderMapper.ToResponse(o));

                return ApiResult<PaginatedResponse<OrderResponse>>.Success(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(GetPagedOrdersAsync));
                return ApiResult<PaginatedResponse<OrderResponse>>.Failure(null, ApiResultError.GenericFailure() );
            }
        }

    }
}
