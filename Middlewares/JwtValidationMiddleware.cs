using AllinOne.Models.Configuration;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.Services.Implementations;
using AllinOne.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace AllinOne.Middlewares
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;
        private readonly IRedisCacheKeyHandlerService _redis;
        private readonly JwtSection _jwtSection;
        private readonly ILogger<JwtService> _logger;
        private readonly IFeatureManagerSnapshot _features;


        public JwtValidationMiddleware(RequestDelegate next
            , IJwtService jwtService,
            IRedisCacheKeyHandlerService redis,
            IOptions<JwtSection> jwtSection,
            ILogger<JwtService> logge,
            IFeatureManagerSnapshot features)//IFeatureManagerSnapshot -> flags must be the same during a request, snapshot locks feature flags for the entire request
        {
            _next = next;
            _jwtService = jwtService;
            _redis = redis;
            _jwtSection = jwtSection.Value;
            _logger = logge;
            _features = features;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!await _features.IsEnabledAsync("JwtMiddlewareEnabled"))
            {
                await _next(context);
                return;
            }

            var requestPath = context.Request.Path.Value?.ToLower();
            try
            {
                if (_jwtSection.WhiteListEndPoints.Any(whitelistPath => 
                requestPath.StartsWith(whitelistPath.ToLower(), StringComparison.OrdinalIgnoreCase)))
                {
                    await _next(context);
                    return;
                }

                var apiKey = context.Request.Headers["ALLINONE-Apikey"].FirstOrDefault();
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Unauthorized request at {Path}: Missing API key or token", requestPath);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Missing API key or token.");
                    return;
                }

                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    _logger.LogWarning("Unauthorized request at {Path}: Invalid or expired token", requestPath);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid or expired token.");
                    return;
                }

                var username = principal.Identity?.Name;
                var key = $"jwt:{username}:{apiKey}";
                var storedToken = await _redis.GetFromRedisCustomKeyAsync<string>(key);

                if (storedToken != token)
                {
                    _logger.LogWarning("Unauthorized request at {Path}: Token not found or mismatched in Redis for user {User}", requestPath, username);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token not found or expired.");
                    return;
                }

                await _redis.RefreshExpirationRedisKeyAsync(key);
                _logger.LogInformation("Validated JWT successfully for user {User} at {Path}", username, requestPath);

                await _next(context);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected error in JwtValidationMiddleware at {Path}", requestPath);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal server error during token validation.");
            }

        }
    }
}
