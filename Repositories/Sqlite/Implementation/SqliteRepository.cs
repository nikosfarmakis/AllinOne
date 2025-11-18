using AllinOne.Constants;
using AllinOne.Data.Sqlite;
using AllinOne.Repositories.Sqlite.Interface;
using AllinOne.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace AllinOne.Repositories.Sqlite.Implementation
{
    public class SqliteRepository<T> : ISqliteRepository<T> where T : class
    {
        protected readonly DbContextSqlite _context;
        protected readonly DbSet<T> _dbSet;
        private readonly ILogger<SqliteRepository<T>> _logger;

        public SqliteRepository(DbContextSqlite context, ILogger<SqliteRepository<T>> logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = logger;
        }


        #region Store
        public virtual async Task<T> StoreAsync(T entity)
        {
            try
            {
                var returnValue = await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created {EntityName} successfully.", typeof(T).Name);
                return (entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(StoreAsync), typeof(T).Name);
                return null;
            }
        }

        public virtual async Task<bool> StoreRangeAsync(IEnumerable<T> storedValue)
        {
            if (storedValue is null || !storedValue.Any())
            {
                _logger.LogWarning("Failed to create range {EntityName} ,Value is null or empty.", typeof(IEnumerable<T>).Name);
                return false;
            }

            try
            {
                await _dbSet.AddRangeAsync(storedValue);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created {EntityName} successfully.", typeof(IEnumerable<T>).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(StoreAsync), typeof(T).Name);
                return false;
            }
        }

        public virtual async Task<bool> StoreRangeAsync(IEnumerable<T> storedValue, int batchSize = 500)
        {
            if (storedValue is null || !storedValue.Any())
            {
                _logger.LogWarning("Failed to create range {EntityName}, value is null or empty.", typeof(T).Name);
                return false;
            }

            try
            {
                foreach (var batch in BatchHelper.SplitIntoBatches(storedValue, batchSize))
                {
                    await _dbSet.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear(); //Deletes all tracked entities
                }

                _logger.LogInformation("Created {EntityName} successfully in batches.", typeof(T).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(StoreRangeAsync), typeof(T).Name);
                return false;
            }
        }

        #endregion

        #region Get
        public virtual async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }
                _logger.LogInformation("Retrieved all {EntityName}", typeof(T).Name);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation}", nameof(GetAllAsync));
                return null;
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }
                _logger.LogInformation("Retrieved entities of {EntityName} with Id: {Id}", typeof(T).Name, id);
                return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id: {Id}", nameof(GetByIdAsync), typeof(T).Name, id);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true)
        {
            IEnumerable<T> result = Enumerable.Empty<T>();
            try
            {
                IQueryable<T> queryable = _dbSet.AsQueryable();

                if (asNoTracking)
                {
                    queryable = queryable.AsNoTracking();
                }

                queryable = queryable.Where(predicate);

                _logger.LogInformation("Retrieved entities of {EntityName} with Predicate: {Predicate}", typeof(T).Name, predicate.ToString());
                result = await queryable.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(GetAsync), typeof(T));
                return result;
            }
        }

        #endregion

        #region Update

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> updatedValue)
        {
            if (updatedValue is null || !updatedValue.Any())
            {
                _logger.LogWarning("Failed to update range {EntityName} ,Value is null or empty.", typeof(IEnumerable<T>).Name);
                return false;
            }

            try
            {
                _dbSet.UpdateRange(updatedValue);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated range {EntityName}", typeof(IEnumerable<T>).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} ", nameof(UpdateRangeAsync), typeof(T).Name);
                return false;
            }
        }

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> updatedValue, int batchSize = 500)
        {
            if (updatedValue is null || !updatedValue.Any())
            {
                _logger.LogWarning("Failed to update range {EntityName}, value is null or empty.", typeof(T).Name);
                return false;
            }

            try
            {
                foreach (var batch in BatchHelper.SplitIntoBatches(updatedValue, batchSize))
                {
                    _dbSet.UpdateRange(batch);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear(); //Deletes all tracked entities
                }

                _logger.LogInformation("Updated {EntityName} in batches.", typeof(T).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(UpdateRangeAsync), typeof(T).Name);
                return false;
            }
        }


        public virtual async Task<InternalDataTransfer<bool>> UpdateAsync(T entity)
        {
            try
            {
                var updatedEntity = _dbSet.Update(entity);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    _logger.LogWarning("Not found {EntityName}", typeof(T).Name);
                    return new InternalDataTransfer<bool>(false,ProjectErrorCodes.NotExisting);
                }

                _logger.LogInformation("Updated {EntityName}", typeof(T).Name);
                return new InternalDataTransfer<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id: {Id}", nameof(UpdateAsync), typeof(T).Name);
                return new InternalDataTransfer<bool>(ex);
            }
        }
        #endregion

        #region Delete
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted {EntityName} successfully with Id: {Id}", typeof(T).Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(DeleteAsync), typeof(T).Name);
                return false;
            }
        }

        public virtual async Task<InternalDataTransfer<bool>> DeleteByIdAsync(Guid id)
        {
            try
            {
                var entity = await GetByIdAsync(id,true);
                if (entity == null)
                {
                    _logger.LogWarning("Not found {EntityName} with Id: {Id} for deletion", typeof(T).Name, id);
                    return new InternalDataTransfer<bool>(false, ProjectErrorCodes.NotExisting);
                }

                _dbSet.Remove(entity);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    _logger.LogInformation("Deleted by id {EntityName} successfully with Id: {Id}", typeof(T).Name, id);
                    return new InternalDataTransfer<bool>(true);
                }

                _logger.LogWarning("Not found {EntityName} with Id: {Id} for deletion", typeof(T).Name, id);
                return new InternalDataTransfer<bool>(false, ProjectErrorCodes.NotExisting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName} with Id {Id}", nameof(DeleteByIdAsync), typeof(T).Name,id);
                return new InternalDataTransfer<bool>(ex);
            }
        }

        public virtual async Task<InternalDataTransfer<int>> DeleteAllAsync()
        {
            try
            {
                var entityType = _context.Model.FindEntityType(typeof(T));
                var tableName = entityType?.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    _logger.LogWarning("Delete All {EntityName} operation failure.", typeof(T).Name);
                    return new InternalDataTransfer<int>(-1,"Table name could not be resolved for entity type.");
                }

                // Dlete all records
                // var deleted = await _context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\";");
                var deleted = await _dbSet.ExecuteDeleteAsync();

                _logger.LogInformation("Deleted All {EntityName} successfully. ", typeof(T).Name);
                return new InternalDataTransfer<int>(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Operation} for {EntityName}", nameof(DeleteAllAsync), typeof(T).Name);
                return new InternalDataTransfer<int>(ex, "Failed to delete all records from table.");
            }
        }

        #endregion

        #region Transaction
        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }
        #endregion

        public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public IQueryable<T> Query() => _dbSet.AsQueryable();


    }
}
