using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IEntityHandlingService<TEntityModel, TModelResponse> where TEntityModel : class
    {
        Task<ApiResult<TModelResponse>> CreateAsync(TEntityModel entity,Func<TEntityModel, TModelResponse> mapResponse);
        Task<ApiResult<TModelResponse>> GetByIdAsync(Guid id, Func<TEntityModel, TModelResponse> mapResponse);
        Task<ApiResult<bool>> DeleteByIdAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<TModelResponse>> UpdateAsync(Guid id, Action<TEntityModel> applyUpdates, Func<TEntityModel, TModelResponse> mapResponse);

    }
}
