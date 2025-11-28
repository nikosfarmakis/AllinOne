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
    public class OrderHandlingService : ModelHandlingService<Order, CreateOrderRequest, OrderResponse, UpdateOrderRequest>,
      IOrderHandlingService
    {
        private readonly IPaginationService<Order> _paginationService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderHandlingService> _logger;
        private readonly IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> _mapper;

        public OrderHandlingService(
            IOrderRepository orderRepository,
            IEntityHandlingService<Order,OrderResponse> entityService,
            IPaginationService<Order> paginationService,
            ILogger<OrderHandlingService> logger,
            IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> mapper) 
            : base(entityService, mapper)
        {
            _orderRepository = orderRepository;
            _paginationService = paginationService;
            _logger = logger;
            _mapper = mapper;
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
                var paged = await _paginationService.ApplyPaginationAsync(
                    query,
                    pagination,
                    filters.SortBy.GetDisplayName(),
                    filters.SortAscending,
                    o => _mapper.ToResponse(o));

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
