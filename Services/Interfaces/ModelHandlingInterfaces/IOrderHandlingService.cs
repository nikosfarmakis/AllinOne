using AllinOne.Models.Requests;
using AllinOne.Models.Requests.OrdrRequests;
using AllinOne.Models.Responses;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces.ModelHandlingServices
{
    public interface IOrderHandlingService : IModelHandlingService<CreateOrderRequest, OrderResponse, UpdateOrderRequest>
    {
        Task<ApiResult<PaginatedResponse<OrderResponse>>> GetPagedOrdersAsync(OrderFilterQuery filters, PaginationQuery pagination);
    }
}
