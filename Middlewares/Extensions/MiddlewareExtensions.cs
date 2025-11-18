using Microsoft.FeatureManagement;

namespace AllinOne.Middlewares.Extensions
{
    public static class MiddlewareExtensions
    {
        // Composite method for call all with line
        public static IApplicationBuilder UsePipelineMiddlewares(this IApplicationBuilder app)
        {
            //var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManagerSnapshot>();
            var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManager>();


            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CorrelationLoggingMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (featureManager.IsEnabledAsync("EnableJwtValidation").Result)
            {
                app.UseMiddleware<JwtValidationMiddleware>();
            }

            return app;
        }

    }
}
