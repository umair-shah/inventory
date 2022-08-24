using Microsoft.EntityFrameworkCore;
using UnitTestExample.Web.Models;

namespace UnitTestExample.Web.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly UnitTestExampleDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(UnitTestExampleDbContext dbContext)
        {
            this._dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            //_dbSet.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}