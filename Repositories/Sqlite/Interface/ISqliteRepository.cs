using AllinOne.ResultPattern;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace AllinOne.Repositories.Sqlite.Interface
{
    public interface ISqliteRepository<T> where T : class
    {
        Task<T> StoreAsync(T entity);
        Task<bool> StoreRangeAsync(IEnumerable<T> storedValue);
        Task<bool> StoreRangeAsync(IEnumerable<T> storedValue, int batchSize);
        Task<bool> UpdateRangeAsync(IEnumerable<T> updatedValue, int batchSize);
        Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true);
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true);
        Task<bool> UpdateRangeAsync(IEnumerable<T> updatedValue);
        Task<InternalDataTransfer<bool>> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<InternalDataTransfer<bool>> DeleteByIdAsync(Guid id);
        Task<InternalDataTransfer<int>> DeleteAllAsync();
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task SaveChangesAsync();
        IQueryable<T> Query();
    }
}
