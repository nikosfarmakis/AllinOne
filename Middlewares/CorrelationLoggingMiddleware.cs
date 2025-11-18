using Serilog.Context;

public class CorrelationLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Items.TryGetValue("CorrelationId", out var correlationIdObj))
        {
            var correlationId = correlationIdObj?.ToString() ?? Guid.NewGuid().ToString();
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
