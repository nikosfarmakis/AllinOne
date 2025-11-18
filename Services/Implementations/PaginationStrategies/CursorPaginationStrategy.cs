using AllinOne.Models;
using AllinOne.Models.Configuration;
using AllinOne.Models.Requests;
using AllinOne.Models.Responses;
using AllinOne.Services.Interfaces.PaginationStrategies;
using AllinOne.Utils.Extensions;
using AllinOne.Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Reflection;

namespace AllinOne.Services.Implementations.PaginationStrategies
{
    public class CursorPaginationStrategy<T, TOut> : IPaginationStrategy<T, TOut>
        where T : class where TOut : class
    {
        private static readonly MethodInfo StringCompareMethod =
            typeof(string).GetMethod(nameof(string.Compare),new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private readonly ILogger<CursorPaginationStrategy<T, TOut>> _logger;
        private readonly PaginationSection _options;
        private readonly PaginationExpressionHelper<T> _exprHelper;
        private readonly IWebHostEnvironment _env;

        public CursorPaginationStrategy(ILogger<CursorPaginationStrategy<T, TOut>> logger,
            IOptions<PaginationSection> options,
            PaginationExpressionHelper<T> exprHelper,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _options = options.Value;
            _exprHelper = exprHelper;
            _env = env;
        }

        public async Task<PaginatedResponse<TOut>> PaginateAsync(
            IQueryable<T> query,
            PaginationQuery pagination,
            string sortBy,
            bool sortAscending,
            Func<T, TOut> selector)
        {
            try
            {
                var totalRecords = await query.CountAsync();

                var sortProp = _exprHelper.GetOrCacheProperty(sortBy, 1800);

                DateTime? cursorCreatedAt = null;
                string? cursorDescription = null;
                Guid? cursorId = null;

                if (!string.IsNullOrEmpty(pagination.After))
                {
                    try
                    {
                        var decoded = System.Text.Json.JsonSerializer.Deserialize<PaginationCursorData>(
                            pagination.After.DecodeFromBase64());

                        cursorCreatedAt = decoded?.TicksDatetime.HasValue == true
                            ? new DateTime(decoded.TicksDatetime.Value, DateTimeKind.Utc)
                            : null;

                        cursorDescription = decoded?.Description;
                        cursorId = decoded?.Id;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error in {Strategy} in {Operation} for entity {EntityName}",
                            nameof(CursorPaginationStrategy<T, TOut>), nameof(PaginateAsync), typeof(T).Name);
                        throw;
                    }
                }

                object? cursorValue = sortProp.PropertyType == typeof(DateTime) ? cursorCreatedAt : cursorDescription;
                query = ApplyDynamicCursorFilter(query, sortProp, sortAscending, cursorValue, cursorId);

                var pageSize = Math.Clamp(pagination.PageSize ?? _options.DefaultPageSize, _options.MinPageSize, _options.MaxPageSize);

                //var items = await query.Take(pageSize).AsNoTracking().ToListAsync();
                var items = await query.Take(pageSize).AsNoTracking().LogAndExecuteAsync(_logger, _env, "Paginate Users");
                var projected = items.Select(selector);

                // Create next cursor
                string? nextCursor = null;
                if (items.Any())
                {
                    var cursorData = CreateNextCursor(items.Last(), sortProp);
                    nextCursor = System.Text.Json.JsonSerializer.Serialize(cursorData).EncodeToBase64();
                }

                return new PaginatedResponse<TOut>(nextCursor, pageSize, totalRecords, projected);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Strategy} in {Operation} for entity {EntityName}",
                            nameof(CursorPaginationStrategy<T, TOut>), nameof(PaginateAsync), typeof(T).Name);
                throw;
            }
        }

        private IQueryable<T> ApplyDynamicCursorFilter(IQueryable<T> query,
            PropertyInfo sortProp,
            bool sortAsc, object? cursorValue,
            Guid? cursorId)
        {
            try
            {
                if (cursorValue == null || cursorId == null)
                {
                    return query;
                }

                var param = Expression.Parameter(typeof(T), "e"); //expression parameter for lambda e =>
                /*                var efPropMethod = typeof(EF).GetMethod("Property", BindingFlags.Static | BindingFlags.Public)!
                                .MakeGenericMethod(sortProp.PropertyType);*/
                var efPropertyMethod = _exprHelper.GetOrCacheEfPropertyMethod(sortProp.PropertyType, 1800);


                // Create Expression.Call MethodCallExpression that calls {Property(e, "CreatedAt")} e.CreatedAt
                // and create EF.Property<DateTime>(e, "CreatedAt") |-> e.CreatedAt
                // this part means "the CreatedAt column of the table"
                var leftExpr = Expression.Call(efPropertyMethod, param, Expression.Constant(sortProp.Name));

                //{31/10/2025 7:00:07 μμ} Constantxpression
                // parameter { Value = 31/10/2025 7:00:07 μμ, Type = DateTime }
                // Constant = parameterized queries -> WHERE CreatedAt > @__cursorValue_0
                var rightExpr = Expression.Constant(cursorValue, sortProp.PropertyType);

                Expression comparison;
                if (sortProp.PropertyType == typeof(string)) //Only string |->string.Compare
                {
                    // use string.Compare
                    var compareCall = Expression.Call(StringCompareMethod, leftExpr, rightExpr);

                    var zero = Expression.Constant(0);
                    //-1 less, 0 equal, +1 greater
                    comparison = sortAsc
                        ? Expression.GreaterThan(compareCall, zero)
                        : Expression.LessThan(compareCall, zero);
                }
                else
                {
                    comparison = sortAsc        //{(Property(e, "CreatedAt") > 31/10/2025 7:00:07 μμ)}
                        ? Expression.GreaterThan(leftExpr, rightExpr)
                        : Expression.LessThan(leftExpr, rightExpr);
                }

                var equalExpr = Expression.Equal(leftExpr, rightExpr);
                var efGuidMethod = typeof(EF).GetMethod("Property", BindingFlags.Static | BindingFlags.Public)!
                    .MakeGenericMethod(typeof(Guid));

                var idExpr = Expression.Call(efGuidMethod, param, Expression.Constant("Id"));   //{Property(e, "Id")}
                var idCompare = sortAsc         //{(Property(e, "Id") > 2cb4669f-fa00-471a-a146-556247351bb3)}
                    ? Expression.GreaterThan(idExpr, Expression.Constant(cursorId.Value))
                    : Expression.LessThan(idExpr, Expression.Constant(cursorId.Value));

                //{((Property(e, "CreatedAt") > 31/10/2025 7:00:07 μμ) OrElse ((Property(e, "CreatedAt") == 31/10/2025 7:00:07 μμ)
                //AndAlso (Property(e, "Id") > 2cb4669f-fa00-471a-a146-556247351bb3)))}

                //WHERE CreatedAt > @cursorValue
                //OR(CreatedAt = @cursorValue AND Id > @cursorId)
                var orExpr = Expression.OrElse(comparison, Expression.AndAlso(equalExpr, idCompare));

                //{e => ((Property(e, "CreatedAt") > 31/10/2025 7:00:07 μμ) OrElse ((Property(e, "CreatedAt") == 31/10/2025 7:00:07 μμ) AndAlso (Property(e, "Id") > 2cb4669f-fa00-471a-a146-556247351bb3)))}
                var lambda = Expression.Lambda<Func<T, bool>>(orExpr, param);

                //System.Linq.IQueryable<AllinOne.Models.SqliteEntities.Order>
                var iQueryable = query.Where(lambda);
                return iQueryable;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Strategy} in {Operation} for entity {EntityName}",
                    nameof(CursorPaginationStrategy<T, TOut>), nameof(ApplyDynamicCursorFilter), typeof(T).Name);
                throw;
            }
        }

        private PaginationCursorData CreateNextCursor(T lastItem, PropertyInfo sortProp)
        {
            try
            {
                var idProp = _exprHelper.GetOrCacheProperty("Id", 1800);

                var lastId = (Guid)idProp.GetValue(lastItem)!;
                long? ticks = null;
                string? desc = null;

                if (sortProp.PropertyType == typeof(DateTime))
                {
                    ticks = ((DateTime)sortProp.GetValue(lastItem)!).Ticks;
                }
                else if (sortProp.PropertyType == typeof(string))
                {
                    desc = (string?)sortProp.GetValue(lastItem);
                }
                return new PaginationCursorData
                {
                    TicksDatetime = ticks,
                    Description = desc,
                    Id = lastId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {Strategy} in {Operation} for entity {EntityName}",
                    nameof(CursorPaginationStrategy<T, TOut>), nameof(CreateNextCursor), typeof(T).Name);
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