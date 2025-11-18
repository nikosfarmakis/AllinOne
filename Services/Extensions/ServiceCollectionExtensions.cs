using AllinOne.MemoryCache.Implementations;
using AllinOne.MemoryCache.Interfaces;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Models.SqliteEntities;
using AllinOne.Redis.Service.Implementations;
using AllinOne.Redis.Service.Interfaces;
using AllinOne.Repositories.Sqlite.Implementation;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.Services.Implementations;
using AllinOne.Services.Implementations.PaginationStrategies;
using AllinOne.Services.Interfaces;
using AllinOne.Services.Interfaces.PaginationStrategies;
using AllinOne.Utils.Helpers;
using AllinOne.Utils.Mappers;
using AllinOne.Utils.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AllinOne.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {

            // Services

            //AddTransient
            #region Mappers
            services.AddTransient<IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest>, OrderMapper>(); 
            //services.AddTransient(typeof(IEntityMapper<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest>),typeof(OrderMapper));
            #endregion

            //Scoped
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));
            services.AddScoped(typeof(IEntityHandlingService<,>), typeof(EntityHandlingService<,>));
            services.AddScoped<IOrderHandlingService, OrderHandlingService>();
            services.AddScoped<IValidationService, ValidationApiKeyService>();
            services.AddScoped<IRedisAdminService, RedisAdminService>();


            #region Pagination Strategies
            services.AddScoped(typeof(IPaginationStrategyFactory<>), typeof(PaginationStrategyFactory<>));

            //services.AddScoped(typeof(OffsetPaginationStrategy<,>)); //PaginationStrategyFactory {3}
            //services.AddScoped(typeof(CursorPaginationStrategy<,>)); //PaginationStrategyFactory {3}

            /// IKeyedServiceProvider -> DI helper interface IKeyedService =>> public interface IKeyedServiceProvider : IServiceProvider
            /// cast IServiceProvider -> IKeyedServiceProvider
            services.AddScoped<IKeyedServiceProvider>(sp => (IKeyedServiceProvider)sp); //ProviderPaginationStrategyFactory {4}
            services.AddKeyedTransient(typeof(IPaginationStrategy<,>), "offset", typeof(OffsetPaginationStrategy<,>)); //PaginationStrategyFactory {4}
            services.AddKeyedTransient(typeof(IPaginationStrategy<,>), "cursor", typeof(CursorPaginationStrategy<,>)); //PaginationStrategyFactory {4}

            services.AddScoped(typeof(PaginationExpressionHelper<>));

            #endregion

            #region repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(ISqliteRepository<>), typeof(SqliteRepository<>));

            #endregion

            #region cache Services
            services.AddSingleton<ICustomCacheKeyHandler, CustomCacheKeyHandler>();
            services.AddSingleton<IRedisCacheKeyHandlerService, RedisCacheKeyHandlerService>();

            #endregion

            //Singleton
            services.AddSingleton<IJwtService, JwtService>();

            //HostedService
            services.AddHostedService<ValidationStartupCheck>();


            return services;
        }
    }
}
