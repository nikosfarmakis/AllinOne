using AllinOne.Models.Responses;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IModelHandlingService<TCreateModelRequest,TModelResponse, TUpdateModelRequest>
    {
        Task<ApiResult<TModelResponse>> CreateModelAsync(TCreateModelRequest request);
        Task<ApiResult<TModelResponse>> GetModelByIdAsync(Guid id);
        Task<ApiResult<bool>> DeleteModelByIdAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<TModelResponse>> UpdateModelAsync(TUpdateModelRequest request);
    }
}
