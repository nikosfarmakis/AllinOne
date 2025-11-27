using AllinOne.Constants;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TModel, TCreateRequest, TUpdateRequest> : ControllerBase where TModel : class
    {
        protected readonly IBaseService<TModel, TCreateRequest, TUpdateRequest> _service;
        protected readonly ILogger _logger;

        protected BaseController(IBaseService<TModel, TCreateRequest, TUpdateRequest> service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create")]
        public virtual async Task<IActionResult> Create([FromBody] TCreateRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                if (result.Status)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<TModel>.Failure(default,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in Create: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result.Status)
                    return Ok(result);

                if (result.Data == null)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<Guid>.Failure(id,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in GetById: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

        [HttpPost("update")]
        public virtual async Task<IActionResult> Update([FromBody] TUpdateRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(request);

                if (result.Status)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<TModel>.Failure(default,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in Update: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (result.Status)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<Guid>.Failure(id,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in Delete: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

        [HttpDelete("deleteAll")]
        public virtual async Task<IActionResult> DeleteAll()
        {
            try
            {
                var result = await _service.DeleteAllAsync();

                if (result.Status)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<int>.Failure(-1,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in DeleteAll: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

        [HttpGet("paged")]
        public virtual async Task<IActionResult> GetPaged([FromQuery] PaginationQuery pagination)
        {
            try
            {
                var result = await _service.GetPagedAsync(pagination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var apiError = ApiResult<PaginatedResponse<TModel>>.Failure(null,
                        new ApiResultError(ProjectErrorCodes.TheActionWasNotPerformed.GetDisplayName()));

                _logger.LogError(ex, "Error in GetPaged: {ApiError}", apiError);
                return BadRequest(apiError);
            }
        }

    }

}
