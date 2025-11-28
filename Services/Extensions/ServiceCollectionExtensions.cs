using AllinOne.Constants;
using AllinOne.MemoryCache.Implementations;
using AllinOne.MemoryCache.Interfaces;
using AllinOne.Models.Requests.OrdrRequests;
using AllinOne.Models.Requests.PatientRequests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteDatabase;
using AllinOne.Models.SqliteEntities;
using AllinOne.Redis.Service.Implementations;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.Repositories.Sqlite.Implementation;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.Services.Implementations;
using AllinOne.Services.Implementations.ModelHandlingServices;
using AllinOne.Services.Implementations.PaginationStrategies;
using AllinOne.Services.Interfaces;
using AllinOne.Services.Interfaces.ModelHandlingServices;
using AllinOne.Services.Interfaces.PaginationStrategies;
using AllinOne.Utils.Helpers;
using AllinOne.Utils.Mappers;
using AllinOne.Utils.Mappers.Interfaces;
using Microsoft.OpenApi.Extensions;

namespace AllinOne.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            //Scoped
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));
            services.AddScoped(typeof(IEntityHandlingService<,>), typeof(EntityHandlingService<,>));
            services.AddScoped<IOrderHandlingService, OrderHandlingService>();
            services.AddScoped<IValidationService, ValidationApiKeyService>();
            services.AddScoped<IRedisAdminService, RedisAdminService>();

            //Singleton
            services.AddSingleton<IJwtService, JwtService>();

            #region Mappers
            //Transient
            services.AddTransient<IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest>, OrderMapper>();
            services.AddTransient<IEntityMapper<Patient, PatientResponse, CreatePatientRequest, UpdatePatientRequest>, PatientMapper>();
        #endregion

        #region Pagination Strategies
        //Scoped
            services.AddScoped(typeof(IPaginationStrategyFactory<>), typeof(PaginationStrategyFactory<>));

            // IKeyedServiceProvider -> DI helper interface IKeyedService =>> public interface IKeyedServiceProvider : IServiceProvider
            // cast IServiceProvider -> IKeyedServiceProvider
            services.AddScoped<IKeyedServiceProvider>(sp => (IKeyedServiceProvider)sp); //ProviderPaginationStrategyFactory {4}
            services.AddKeyedScoped(typeof(IPaginationStrategy<,>), PaginationStrategiesCategs.Offset.GetDisplayName(), typeof(OffsetPaginationStrategy<,>)); //PaginationStrategyFactory {4}
            services.AddKeyedScoped(typeof(IPaginationStrategy<,>), PaginationStrategiesCategs.Cursor.GetDisplayName(), typeof(CursorPaginationStrategy<,>)); //PaginationStrategyFactory {4}
            //services.AddScoped(typeof(OffsetPaginationStrategy<,>)); //PaginationStrategyFactory {3}
            //services.AddScoped(typeof(CursorPaginationStrategy<,>)); //PaginationStrategyFactory {3}
            services.AddScoped(typeof(PaginationExpressionHelper<>));
            #endregion

            #region repositories
            //Scoped
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(ISqliteRepository<>), typeof(SqliteRepository<>));
            #endregion

            #region cache Services
            //Singleton
            services.AddSingleton<ICustomCacheKeyHandler, CustomCacheKeyHandler>();
            services.AddSingleton<IRedisCacheKeyHandlerService, RedisCacheKeyHandlerService>();
            #endregion

            #region HostedService
            services.AddHostedService<ValidationStartupCheck>();
            #endregion

            return services;
        }
    }
}
