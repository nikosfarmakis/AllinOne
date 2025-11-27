using AllinOne.Models.Responses;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IModelHandlingService<TCreateModelRequest,TResponse, TUpdateModelRequest>
    {
        Task<ApiResult<TResponse>> CreateModelAsync(TCreateModelRequest request);
        Task<ApiResult<TResponse>> GetModelByIdAsync(Guid id);
        Task<ApiResult<bool>> DeleteModelByIdAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<TResponse>> UpdateModelAsync(TUpdateModelRequest request);
    }
}
