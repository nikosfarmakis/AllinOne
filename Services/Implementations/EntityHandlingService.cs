using AllinOne.Constants;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Services.Implementations
{
    public class EntityHandlingService<TEntity, TResponse> : IEntityHandlingService<TEntity, TResponse> where TEntity : class
    {
        private readonly ISqliteRepository<TEntity> _repository;
        private readonly ILogger<EntityHandlingService<TEntity, TResponse>> _logger;

        public EntityHandlingService(ISqliteRepository<TEntity> repository , ILogger<EntityHandlingService<TEntity, TResponse>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResult<TResponse>> CreateAsync(TEntity entity, Func<TEntity, TResponse> mapResponse)
        {
            try
            {
                var result = await _repository.StoreAsync(entity);

                if (result == null)
                {
                    _logger.LogWarning("Failed to create {EntityName}.", typeof(TEntity).Name);
                    return ApiResult<TResponse>.Failure(default, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
                }

                // mapResponse = delegate entity -> response
                var response = mapResponse(result);

                _logger.LogInformation("Created {EntityName} successfully.", typeof(TEntity).Name);
                return ApiResult<TResponse>.Success(response, $"{typeof(TEntity).Name} created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(CreateAsync), typeof(TEntity).Name);
                return ApiResult<TResponse>.Failure(default, new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));
            }
        }

        public async Task<ApiResult<TResponse>> GetByIdAsync(Guid id, Func<TEntity, TResponse> mapResponse)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id, true);
                if (result == null)
                {
                    _logger.LogWarning("Νot found {EntityName} with Id: {Id}", typeof(TEntity).Name, id);
                    return ApiResult<TResponse>.Failure(default, ApiResultError.NotFound());
                }
                var response = mapResponse(result);
                _logger.LogInformation("Retrieved entity of {EntityName} with Id: {Id}", typeof(TEntity).Name, id);
                return ApiResult<TResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id {Id}", nameof(GetByIdAsync), typeof(TEntity).Name, id);
                return ApiResult<TResponse>.Failure(default, ApiResultError.GenericFailure());
            }
        }

        public async Task<ApiResult<bool>> DeleteByIdAsync(Guid id)
        {
            try
            {
                var result = await _repository.DeleteByIdAsync(id);

                if (result.Data)
                {
                    _logger.LogInformation("Deleted {EntityName} successfully with Id: {Id}", typeof(TEntity).Name, id);
                    return ApiResult<bool>.Success(true, $"{typeof(TEntity).Name} deleted successfully");
                }

                if (result.ProjectErrorCodes == ProjectErrorCodes.NotExisting)
                {
                    _logger.LogWarning("Νot found {EntityName} with Id: {Id} for deletion", typeof(TEntity).Name, id);
                    return ApiResult<bool>.Failure(false, ApiResultError.NotFound());
                }
                _logger.LogWarning("Delete operation failed for {EntityName} with Id: {Id}", typeof(TEntity).Name, id);
                return ApiResult<bool>.Failure(false, ApiResultError.GenericFailure());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id: {Id}", nameof(DeleteByIdAsync), typeof(TEntity).Name, id);
                return ApiResult<bool>.Failure(false, ApiResultError.GenericFailure());
            }
        }

        public async Task<ApiResult<int>> DeleteAllAsync()
        {
            try
            {
                var result = await _repository.DeleteAllAsync();

                if (result.Data>=0)
                {
                    _logger.LogInformation("Deleted All {EntityName} successfully. ", typeof(TEntity).Name);
                    return ApiResult<int>.Success(result.Data, $"{typeof(TEntity).Name} All deleted successfully");
                }
                _logger.LogWarning("Delete All {EntityName} operation failure.", typeof(TEntity).Name);
                return ApiResult<int>.Failure(-1, ApiResultError.GenericFailure());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}.", nameof(DeleteAllAsync), typeof(TEntity).Name);
                return ApiResult<int>.Failure(-1, ApiResultError.GenericFailure());
            }
        }

        public async Task<ApiResult<TResponse>> UpdateAsync(Guid id, Action<TEntity> applyUpdates, Func<TEntity, TResponse> mapResponse)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id, true);
                if (entity == null)
                {
                    _logger.LogWarning("Not found {EntityName} with Id: {Id} for update", typeof(TEntity).Name, id);
                    return ApiResult<TResponse>.Failure(default, ApiResultError.NotFound());
                }

                applyUpdates(entity); // p.x. request.UpdateToOrder(entity)
                var success = await _repository.UpdateAsync(entity);
                if (success.Data)
                {
                    _logger.LogInformation("Updated {EntityName} successfully with Id: {Id}", typeof(TEntity).Name, id);
                    return ApiResult<TResponse>.Success(mapResponse(entity), $"{typeof(TEntity).Name} updated successfully");
                }
                _logger.LogWarning("Failed to {Operation} {EntityName} with Id: {Id}", nameof(UpdateAsync), typeof(TEntity).Name, id);
                return ApiResult<TResponse>.Failure(default, ApiResultError.GenericFailure());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id: {Id}", nameof(UpdateAsync), typeof(TEntity).Name, id);
                return ApiResult<TResponse>.Failure(default, ApiResultError.GenericFailure());
            }
        }
    }
}
