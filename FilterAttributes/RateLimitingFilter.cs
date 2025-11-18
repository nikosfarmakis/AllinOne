using AllinOne.MemoryCache.Interfaces;
using AllinOne.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AllinOne.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RateLimitingFilter : Attribute, IAsyncActionFilter
    {
        private readonly int _maxRequests;
        private readonly int _expirationInSeconds;

        public RateLimitingFilter(int maxRequests = 100, int expirationInSeconds = 120)
        {

            _maxRequests = maxRequests;
            _expirationInSeconds = expirationInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var cache = context.HttpContext.RequestServices.GetService<ICustomCacheKeyHandler>();
                if (cache == null)
                {
                    context.Result = BuildRequestResponse("Cache service not available.");
                    return;
                }

                var controllerIdentifier = context.ActionDescriptor.RouteValues["controller"] ?? string.Empty;
                var endpointIdentifier = context.ActionDescriptor.RouteValues["action"] ?? string.Empty;

                RateLimitingFilter currentThisFilter = (RateLimitingFilter)context.Filters.First(f => f == this);
                IList<FilterDescriptor> allFiltersDescr = context.ActionDescriptor.FilterDescriptors;

                bool isControllerLevel =
                    allFiltersDescr.Any(fDesc => fDesc.Filter == currentThisFilter && fDesc.Scope == FilterScope.Controller);

                string? key = isControllerLevel
                       ? $"controller:{controllerIdentifier}_rate_limit"
                       : $"endpoint:{endpointIdentifier}:{endpointIdentifier}_rate_limit";

                if (ShouldRateLimit(key!, cache))
                {
                    context.Result = BuildRequestResponse(isControllerLevel
                        ? "Controller rate limit exceeded"
                        : "Endpoint rate limit exceeded");
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = BuildRequestResponse($"Rate limit check failed: {ex.Message}");
            }

        }

        public bool ShouldRateLimit(string cacheKey, ICustomCacheKeyHandler cache)
        {
            if (cache.TryRetrieveKey<RateLimitingCacheModel>(cacheKey, out var rateLimitingCacheModel) && rateLimitingCacheModel != null)
            {
                if ( rateLimitingCacheModel.CheckAvailableRequestsAndIncrementCounter())
                {
                    cache.StoreInMemoryAbsoluteCustomKey(cacheKey, rateLimitingCacheModel, _expirationInSeconds); // update
                    return false;
                }
                return true;
            }

            cache.StoreInMemoryAbsoluteCustomKey(cacheKey,  new RateLimitingCacheModel(_maxRequests , _expirationInSeconds), _expirationInSeconds);  // store new
            return false;
        }

        private IActionResult BuildRequestResponse(string message)
            => new BadRequestObjectResult(new { message });

    }
}
