using AllinOne.Data.Sqlite;
using AllinOne.Models.SqliteEntities;
using AllinOne.Repositories.Sqlite.Interface;

namespace AllinOne.Repositories.Sqlite.Implementation
{
    public class OrderRepository : SqliteRepository<Order>, IOrderRepository
    {
        public OrderRepository(DbContextSqlite context, ILogger<OrderRepository> logger) : base(context, logger)
        {

        }
    }

/*        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _context.Orders.ToListAsync();

        public async Task<Order?> GetByIdAsync(Guid id)
            => await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        public async Task AddAsync(Order order)
            => await _context.Orders.AddAsync(order);

        public async Task UpdateAsync(Order order)
            => _context.Orders.Update(order);

        public async Task DeleteAsync(Order order)
            => _context.Orders.Remove(order);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();*/
    
}
