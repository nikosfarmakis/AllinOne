using AllinOne.Constants;
using AllinOne.Models.Configuration;
using AllinOne.Models.Requests.Authorization;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.ResultPattern;
using AllinOne.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Services.Implementations
{
    public class ValidationApiKeyService : IValidationService
    {
        private readonly AccessSection _accessSection;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly IRedisCacheKeyHandlerService _redis;
        private readonly ILogger<JwtService> _logger;

        public ValidationApiKeyService(IOptions<AccessSection> options,//singleton
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings,
            IRedisCacheKeyHandlerService redis,
            ILogger<JwtService> logger
            )
        {
            _accessSection = options.Value;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _redis = redis;
            _logger = logger;
        }

        public async Task<ApiResult<string?>> ValidUser(LoginRequest request, string apiKey)
        {
            try
            {
                var user = _accessSection.UsersWithAccess.Values.FirstOrDefault(u => u.Username == request.Username &&
                        u.Password == request.Password && u.Apikey == apiKey);

                if (user is null)
                {
                    _logger.LogWarning("Invalid credentials or API key. Username: {username}", request.Username);
                    return ApiResult<string?>.Failure(null, new ApiResultError("Invalid credentials or API key."));
                }

                var token = _jwtService.GenerateToken(user.Username, apiKey);

                var t = await _redis.StoreOrUpdateRedisExpirationCustomKeyAsync(
                   $"jwt:{user.Username}:{apiKey}", token, _jwtSettings.ExpirationMinutes * 60);

                _logger.LogInformation("Creating token for User: {username}", request.Username);
                return ApiResult<string?>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(ValidUser));
                return ApiResult<string?>.Failure(null, new ApiResultError(ProjectErrorCodes.InternalError.GetDisplayName()),true);
            }
        }
    }
}


        /*        public bool IsApiKeyValid(string apiKey)
                {
                    return _accessSection.UsersWithAccess.Values
                        .Any(user => user.Apikey == apiKey);
                }

                public UserWithAccess? GetUserByApiKey(string apiKey)
                {
                    return _accessSection.UsersWithAccess.Values
                        .FirstOrDefault(user => user.Apikey == apiKey);
                }*/