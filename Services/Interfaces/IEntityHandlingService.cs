using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteEntities;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IEntityHandlingService<TEntity, TResponse> where TEntity : class
    {
        Task<ApiResult<TResponse>> CreateAsync(TEntity entity,Func<TEntity, TResponse> mapResponse);
        Task<ApiResult<TResponse>> GetByIdAsync(Guid id, Func<TEntity, TResponse> mapResponse);
        Task<ApiResult<bool>> DeleteByIdAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<TResponse>> UpdateAsync(Guid id, Action<TEntity> applyUpdates, Func<TEntity, TResponse> mapResponse);

    }
}
