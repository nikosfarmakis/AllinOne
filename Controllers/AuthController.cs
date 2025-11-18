using AllinOne.Constants;
using AllinOne.Models.Configuration;
using AllinOne.Models.Requests.Authorization;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.ResultPattern;
using AllinOne.Services.Implementations;
using AllinOne.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IValidationService _validationService;


        public AuthController(
            IValidationService validationService)
        {
            _validationService = validationService;
        }

        [ProducesResponseType(typeof(ApiResult<string?>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<string?>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string?>), StatusCodes.Status400BadRequest)]
        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var apiKey = Request.Headers["X-ApiKey"].FirstOrDefault();
            if (apiKey == null)
            {
                return Unauthorized(ApiResult<string?>.Failure(null, new ApiResultError(ProjectErrorCodes.MissingAPIkey.GetDisplayName())));
            }

            var apiResult = await _validationService.ValidUser(request, apiKey);

            return apiResult switch
            {
                { Status: true } => Ok(apiResult),
                { HasException: false } => Unauthorized(apiResult),
                _ => BadRequest(apiResult)
            };
        }
    }
}
