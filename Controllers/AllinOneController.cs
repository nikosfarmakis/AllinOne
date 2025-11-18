using AllinOne.FilterAttributes;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteEntities;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using AllinOne.Utils.Mappers;
using AllinOne.Utils.Mappers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllinOne.Controllers
{
    [RateLimitingFilter(20, expirationInSeconds: 20)]
    [ApiController]
    [Route("api/[controller]")]
    public class AllinOneController : ControllerBase
    {

        private readonly IRedisCacheKeyHandlerService _redisCache;
        private readonly ILogger<AllinOneController> _logger;
        private readonly IOrderRepository _repository;
        private readonly IOrderHandlingService _orderPaginationService;
        private readonly IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> _orderMapper;

        public AllinOneController(ILogger<AllinOneController> logger, IOrderRepository repository,
            IRedisCacheKeyHandlerService redisCache, IOrderHandlingService orderPaginationService,
            IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest> orderMapper)
        {
            _logger = logger;
            _repository = repository;
            _redisCache = redisCache;
            _orderPaginationService = orderPaginationService;
            _orderMapper = orderMapper;
        }

        [RateLimitingFilter(2, expirationInSeconds: 2000)]
        [HttpPost("testOrder")]
        //[ProducesResponseType(typeof(ApiResult<ResponseModel>), StatusCodes.Status201Created)]

        public async Task<IActionResult> CreateOrder([FromBody] int? AuthId)
        {
            Console.WriteLine($"[Start] ThreadId={Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(1000); 

            Console.WriteLine($"[After Delay] ThreadId={Thread.CurrentThread.ManagedThreadId}");
            //throw new Exception("TestError Descr");

            return Ok("All OK 200");
        }

        [HttpGet("get_repository.GetAllAsync")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _repository.GetAllAsync();
            return Ok(orders);
        }

        [HttpPost("createOrder")]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _repository.StoreAsync(_orderMapper.ToEntity(request));
                await _repository.SaveChangesAsync();
                var result = ApiResult<OrderResponse>.Success(_orderMapper.ToResponse(order), "Order created");

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString()); 
            }
        }

        [HttpGet("{id}")]
        //[HttpGet("getOrderById")]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);
            return order == null ? NotFound() : Ok(_orderMapper.ToResponse(order));
        }

        [HttpPost("setRedis")]
        public async Task<IActionResult> SetRedis()
        {
            await _redisCache.StoreOrUpdateInRedisPermanentKeyAsync(Guid.NewGuid().ToString(), new { Name = "Nikos" });
            return Ok("Stored!");
        }

        [HttpGet("getRedis")]
        public async Task<IActionResult> GetRedis()
        {
            var user = await _redisCache.GetFromRedisCustomKeyAsync<dynamic>("testkey");
            return Ok(user ?? "Not found");
        }
/*
        [HttpGet("orderPagination")]
        public async Task<IActionResult> GetPagedOrders([FromQuery] PaginationQuery query)
        {
            var result = await _orderPaginationService.GetPagedOrdersAsync(query);
            return Ok(result);
        }*/
    }
}
