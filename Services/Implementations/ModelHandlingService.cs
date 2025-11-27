using AllinOne.Models.Requests;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using AllinOne.Utils.Mappers.Interfaces;

namespace AllinOne.Services.Implementations
{
    public class ModelHandlingService<TEntityModel, TCreateModelRequest, TModelResponse, TUpdateModelRequest> :
        IModelHandlingService<TCreateModelRequest, TModelResponse, TUpdateModelRequest> where TEntityModel : class
        where TUpdateModelRequest : IUpdateRequest
    {
        private readonly IEntityHandlingService<TEntityModel, TModelResponse> _entityService;
        private readonly IEntityMapper<TEntityModel, TModelResponse, TCreateModelRequest, TUpdateModelRequest> _entityMapper;

        public ModelHandlingService(IEntityHandlingService<TEntityModel, TModelResponse> entityService,
            IEntityMapper<TEntityModel, TModelResponse, TCreateModelRequest, TUpdateModelRequest> entityMapper)
        {
            _entityService = entityService;
            _entityMapper = entityMapper;
        }
        public Task<ApiResult<TModelResponse>> CreateModelAsync(TCreateModelRequest request)
        {
            var entity = _entityMapper.ToEntity(request);
            return _entityService.CreateAsync(entity, o => _entityMapper.ToResponse(o));
        }

        public Task<ApiResult<TModelResponse>> GetModelByIdAsync(Guid id)
        {
            return _entityService.GetByIdAsync(id, o => _entityMapper.ToResponse(o));
        }

        public Task<ApiResult<bool>> DeleteModelByIdAsync(Guid id)
        {
            return _entityService.DeleteByIdAsync(id);
        }

        public Task<ApiResult<int>> DeleteAllAsync()
        {
            return _entityService.DeleteAllAsync();
        }

        public Task<ApiResult<TModelResponse>> UpdateModelAsync(TUpdateModelRequest request)
        {
            //entity -> Order from UpdateToOrder
            //public static void UpdateToOrder(this UpdateOrderRequest request, Order entity)]
            return _entityService.UpdateAsync(request.Id, entity => _entityMapper.UpdateEntity(request, entity), o => _entityMapper.ToResponse(o));
        }

    }
}
