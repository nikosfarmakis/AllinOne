using AllinOne.Constants;
using AllinOne.FilterAttributes;
using AllinOne.Models.Requests;
using AllinOne.Models.Requests.OrdrRequests;
using AllinOne.Models.Responses;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RateLimitingFilter(30, expirationInSeconds: 20)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderHandlingService _orderHandlingService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderHandlingService orderHandlingService, ILogger<OrderController> logger)
        {
            _orderHandlingService = orderHandlingService;
            _logger = logger;
        }


        [HttpPost("createOrder")]
        [RateLimitingFilter(10, expirationInSeconds: 200)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var apiResult = await _orderHandlingService.CreateOrderAsync(request);

                if (apiResult.Status)
                {
                    return CreatedAtAction(nameof(GetOrderById), new { id = apiResult.Data.Id }, apiResult);
                }
                return BadRequest(apiResult);

            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<OrderResponse>.Failure(null, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(CreateOrder), apiResultError);
                return BadRequest(apiResultError);
            }
        }

        [HttpGet("{id}")]
        [RateLimitingFilter(10, expirationInSeconds: 200)]
        //[HttpGet("getOrderById")]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var apiResult = await _orderHandlingService.GetOrderByIdAsync(id);
                if (apiResult.Status)
                {
                    return Ok(apiResult);
                }

                if (apiResult.Data == null)
                {
                    return NotFound(apiResult);
                }

                return BadRequest(apiResult);
            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<Guid>.Failure(id, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(GetOrderById), apiResultError);
                return BadRequest(apiResultError);
            }
        }

        [HttpDelete("{id}")]
        [RateLimitingFilter(10, expirationInSeconds: 200)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderById(Guid id)
        {
            try
            {
                var apiResult = await _orderHandlingService.DeleteOrderByIdAsync(id);
                if (apiResult.Status)
                {
                    return Ok(apiResult);
                }

                if (apiResult.Error?.ErrorDescription == ProjectErrorCodes.NotExisting.GetDisplayName())
                {
                    return NotFound(apiResult);
                }

                return BadRequest(apiResult);
            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<Guid>.Failure(id, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(DeleteOrderById), apiResultError);
                return BadRequest(apiResultError);
            }
        }
        [HttpDelete("deleteAll")]
        [RateLimitingFilter(20, expirationInSeconds: 200)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteOrderAll()
        {
            try
            {
                var apiResult = await _orderHandlingService.DeleteAllAsync();
                if (apiResult.Status)
                {
                    return Ok(apiResult);
                }

                return BadRequest(apiResult);
            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<int>.Failure(-1, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(DeleteOrderAll), apiResultError);
                return BadRequest(apiResultError);
            }
        }

        [HttpPost("updateOrder")]
        [RateLimitingFilter(10, expirationInSeconds: 200)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
        {
            try
            {
                var apiResult = await _orderHandlingService.UpdateOrderAsync(request);
                if (apiResult.Status)
                {
                    return Ok(apiResult);
                }

                if (apiResult.Error?.ErrorDescription == ProjectErrorCodes.NotExisting.GetDisplayName())
                {
                    return NotFound(apiResult);

                }
                return BadRequest(apiResult);

            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<OrderResponse>.Failure(null, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(UpdateOrder), apiResultError);
                return BadRequest(apiResultError);
            }
        }

        [HttpGet("orderPagination")]
        [RateLimitingFilter(20, expirationInSeconds: 200)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<OrderResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedOrders([FromQuery] PaginationQuery pagination, [FromQuery] OrderFilterQuery orderFilter)
        {
            try
            {
                var result = await _orderHandlingService.GetPagedOrdersAsync(orderFilter, pagination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var apiResultError = ApiResult<PaginatedResponse<OrderResponse>>.Failure(null, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                _logger.LogError(ex, "Error in {Operation}, {ApiResult}", nameof(GetPagedOrders), apiResultError);
                return BadRequest(apiResultError);
            }
        }

    }
}
