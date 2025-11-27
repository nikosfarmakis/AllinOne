using Microsoft.FeatureManagement;

namespace AllinOne.Middlewares.Extensions
{
    public static class MiddlewareExtensions
    {
        // Composite method for call all with line
        public static IApplicationBuilder UsePipelineMiddlewares(this IApplicationBuilder app) //sync
        {
            //var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManagerSnapshot>();
            var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManager>();


            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CorrelationLoggingMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            var result = featureManager.IsEnabledAsync("EnableJwtValidation");

            // .GetAwaiter().GetResult() -> async to sync.
            // safe way to call async code synchronously, because it directly returns the actual exception and not AggregateException
            // you will see exactly the exception thrown by the async method
            if (featureManager.IsEnabledAsync("EnableJwtValidation").GetAwaiter().GetResult()) 
            {
                app.UseMiddleware<JwtValidationMiddleware>();
            }

            return app;
        }

    }
}
