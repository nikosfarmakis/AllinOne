using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteEntities;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IOrderHandlingService
    {
        Task<ApiResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request);
        Task<ApiResult<OrderResponse>> GetOrderByIdAsync(Guid id);
        Task<ApiResult<bool>> DeleteOrderByIdAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<OrderResponse>> UpdateOrderAsync(UpdateOrderRequest request);
        Task<ApiResult<PaginatedResponse<OrderResponse>>> GetPagedOrdersAsync(OrderFilterQuery filters, PaginationQuery pagination);

    }
}
