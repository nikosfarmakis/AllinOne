using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.ResultPattern;

namespace AllinOne.Services.Interfaces
{
    public interface IBaseService<TModel, TCreateRequest, TUpdateRequest> where TModel : class
    {
        Task<ApiResult<TModel>> CreateAsync(TCreateRequest request);
        Task<ApiResult<TModel>> GetByIdAsync(Guid id);
        Task<ApiResult<TModel>> UpdateAsync(TUpdateRequest request);
        Task<ApiResult<bool>> DeleteAsync(Guid id);
        Task<ApiResult<int>> DeleteAllAsync();
        Task<ApiResult<PaginatedResponse<TModel>>> GetPagedAsync(PaginationQuery pagination);
    }
}
