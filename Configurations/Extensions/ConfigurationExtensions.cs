using AllinOne.Configurations.Validators;
using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Configurations.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration) //default appsettings.json
                .AddJsonFile("ApikeysAndJwtSettings.json", optional: true, reloadOnChange: true)             // add ApikeysAndJwtSettings.json
                .Build();


            //StaticConfigurations

            //services.Configure<JwtSettings>(configBuilder.GetSection("JwtSettings"));
            services.AddOptions<JwtSettings>().Bind(configBuilder.GetSection("JwtSettings"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            
            //services.Configure<PaginationSection>(configBuilder.GetSection("PaginationSection"));
            services.AddOptions<PaginationSection>()
                    .Bind(configBuilder.GetSection("PaginationSection"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            //DynamicConfigurations

            services.Configure<AccessSection>(configBuilder.GetSection("AccessSection"));
            // Validation =>  UsersWithAccess| IOptionsMonitor<AccessSection> which is a singleton
            services.AddSingleton<IValidateOptions<AccessSection>, UsersWithAccessValidator>(); 


            return services;
        }


    }
}
