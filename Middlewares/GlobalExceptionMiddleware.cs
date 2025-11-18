using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AllinOne.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // run next middlewares/controllers
            }
            catch (Exception ex)
            {
                // Log exception with correlationId
                _logger.LogError(ex, "Unhandled exception for request {Method} {Path} {Message} {Τrace}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message,
                    ex.StackTrace);

                // return JSON response to the client
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    status = context.Response.StatusCode,
                    exeption = ex.Message,
                    trace = ex.StackTrace,
                    message = "Internal server error",
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
