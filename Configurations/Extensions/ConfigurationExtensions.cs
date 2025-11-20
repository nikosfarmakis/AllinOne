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
                .AddJsonFile("Configurations/ApikeysAndJwtSettings.json", optional: false, reloadOnChange: true) 
                .AddJsonFile("Configurations/UserPasswordSettings.json", optional: false, reloadOnChange: false)
                .Build();


            //StaticConfigurations

            //services.Configure<JwtSettings>(configBuilder.GetSection("JwtSettings"));
            services.AddOptions<JwtSection>().Bind(configBuilder.GetSection("JwtSettings")).ValidateDataAnnotations().ValidateOnStart();

            //services.Configure<PaginationSection>(configBuilder.GetSection("PaginationSection"));
            services.AddOptions<PaginationSection>()
                    .Bind(configBuilder.GetSection("PaginationSettings")).ValidateDataAnnotations().ValidateOnStart();

            services.AddOptions<UserPasswordSection>()
                    .Bind(configBuilder.GetSection("UserPasswordSettings")).ValidateDataAnnotations().ValidateOnStart();

            //DynamicConfigurations

            services.Configure<AccessSection>(configBuilder.GetSection("AccessSettings"));
            // Validation =>  UsersWithAccess| IOptionsMonitor<AccessSection> which is a singleton
            services.AddSingleton<IValidateOptions<AccessSection>, UsersWithAccessValidator>(); 


            return services;
        }


    }
}
