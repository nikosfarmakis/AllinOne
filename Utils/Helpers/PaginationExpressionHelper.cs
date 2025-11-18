using AllinOne.MemoryCache.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace AllinOne.Utils.Helpers
{
    public class PaginationExpressionHelper<T> where T : class
    {
        private readonly ICustomCacheKeyHandler _cache;
        private readonly ILogger<PaginationExpressionHelper<T>> _logger;


        public PaginationExpressionHelper(ICustomCacheKeyHandler cache, ILogger<PaginationExpressionHelper<T>> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, string propertyName, bool ascending)
        {
            try
            {                
                var typeT = typeof(T);
                // var cachedLambda = GetOrCreateLambda( propertyName); //{x => x.CreatedAt}
                var cachedProp = GetOrCacheProperty(propertyName, 1800); //{System.DateTime CreatedAt}

                var param = Expression.Parameter(typeT, "x"); //{x}
                                                              // Convert check that the result is of the correct type
                var body = Expression.Convert(Expression.PropertyOrField(param, propertyName), cachedProp.PropertyType); //{Convert(x.CreatedAt, DateTime)}
                //var dynamicLambda = Expression.Lambda(body, param); //{x => Convert(x.CreatedAt, DateTime)}
                var dynamicLambda = GetOrCacheOrderingLambda(propertyName, 1800);


                string methodName = ascending ? "OrderBy" : "OrderByDescending";

                var resultExpr = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeT, cachedProp.PropertyType },
                    query.Expression,
                    Expression.Quote(dynamicLambda)
                );

                return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpr);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Operation} for entity {EntityName}",
                    nameof(ApplyOrdering), typeof(T).Name);
                throw;
            }
        }
        public PropertyInfo GetOrCacheProperty(string propertyName, int sec)
        {
            try
            {
                var cacheKey = $"{typeof(T).FullName}:{propertyName}:prop";

                if (_cache.TryRetrieveKey<PropertyInfo>(cacheKey, out var prop))
                {
                    return prop;
                }

                //prop = typeof(T).GetRuntimeProperty(propertyName); //does not have BindingFlags.IgnoreCase
                prop = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) //{System.DateTime CreatedAt}
                    ?? throw new InvalidOperationException($"Property '{propertyName}' not found on type {typeof(T).Name}");

                _cache.StoreInMemorySlidingCustomKey(cacheKey, prop, sec);
                return prop;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Operation} for entity {EntityName}",
                    nameof(GetOrCacheProperty), typeof(T).Name);
                throw;
            }
        }
        public LambdaExpression GetOrCacheOrderingLambda(string propertyName, int sec)
        {
            try
            {
                var cacheKey = $"{typeof(T).FullName}:{propertyName}:orderingLambda";

                if (_cache.TryRetrieveKey<LambdaExpression>(cacheKey, out var lambda))
                {
                    return lambda;
                }

                var typeT = typeof(T);
                var prop = GetOrCacheProperty(propertyName, sec);
                var param = Expression.Parameter(typeT, "x");
                var body = Expression.Convert(Expression.PropertyOrField(param, propertyName), prop.PropertyType);
                lambda = Expression.Lambda(body, param);

                _cache.StoreInMemorySlidingCustomKey(cacheKey, lambda, sec);
                return lambda;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Operation} for entity {EntityName}",
                    nameof(GetOrCacheOrderingLambda), typeof(T).Name);
                throw;

            }
        }
        public MethodInfo GetOrCacheEfPropertyMethod(Type propertyType, int sec)
        {
            try
            {
                var cacheKey = $"EF.Property:{propertyType.FullName}";
                if (_cache.TryRetrieveKey<MethodInfo>(cacheKey, out var method))
                {
                    return method;
                }

                var baseMethod = typeof(EF).GetMethod("Property", BindingFlags.Static | BindingFlags.Public)!;
                method = baseMethod.MakeGenericMethod(propertyType);

                _cache.StoreInMemorySlidingCustomKey(cacheKey, method, sec);
                return method;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(GetOrCacheEfPropertyMethod), typeof(T).Name);
                throw;
            }
        }
    }
}
/*        GetMethod - locates a method through reflection(runtime)
                    MakeGenericMethod - specializes the generic method(e.g.EF.Property<DateTime>)
                    Expression.Call - Creates an expression node that represents a method call
                    Expression.Constant - Creates a literal/parameter (constant value)
                    Expression.Parameter  - Creates "e" (lambda parameter)
                    Expression.GreaterThan/LessThan - Creates a comparative expression (> <)
                    Expression.OrElse / AndAlso - Creates logical conditions(OR AND)
                    Expression.Lambda - Creates a final lambda(px e => ...)
                    query.Where(lambda) - Applies the expression to IQueryable*/