using System.Linq.Expressions;
using Company.Project.Domain.Interfaces;
using Company.Project.Infrastructure.Filter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Company.Project.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly Context _dbContext;
        public BaseRepository(Context context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Actions
        
        public virtual async Task<T> GetByIdAsync(int id) =>
            await _dbContext.Set<T>().FindAsync(id);


        public IQueryable<T> GetTableNoTracking()=>
            _dbContext.Set<T>().AsNoTracking().AsQueryable();


        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

        }
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();

        }
        public async Task<IEnumerable<T>> GetAllAsync(int Skip, int Take)
            => await _dbContext.Set<T>().Skip(Skip).Take(Take).ToListAsync();
        public async Task<IEnumerable<T>> GetAllAsync( Expression<Func<T, object>>[] includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.ToListAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var property = typeof(T).GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
                _dbContext.Set<T>().Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            else 
            {
                _dbContext.Set<T>().Remove(entity);
                throw new InvalidOperationException("The entity does not have an 'IsDeleted' property.");
            }            
        }
        public virtual async Task HardDeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        
        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (T entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()=>
            await _dbContext.SaveChangesAsync();



        public IDbContextTransaction BeginTransaction() =>
            _dbContext.Database.BeginTransaction();

        public void Commit()=>
            _dbContext.Database.CommitTransaction();

        public void RollBack()=>
            _dbContext.Database.RollbackTransaction();

        public IQueryable<T> GetTableAsTracking()=> _dbContext.Set<T>().AsQueryable();


        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbContext.Set<T>().ToListAsync();
        public async Task<IEnumerable<T>> GetAllFilteredAsync(string[] filters)
        {
            var entity = _dbContext.Set<T>();
        
            var result = DynamicFilterHelper.ApplyFilters(entity,filters);
        
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByNameAsync(Expression<Func<T, bool>> expression, string name)
         => await _dbContext.Set<T>().Where(expression).ToListAsync();

        public async Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression,Expression<Func<T, object>>[] includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.Where(expression).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetByExpressionAsync(int Skip, int Take, Expression<Func<T, bool>> expression)
          => await _dbContext.Set<T>().Where(expression).Skip(Skip).Take(Take).ToListAsync();
        public async Task<int> CountAsync(Expression<Func<T, bool>>? expression = default)
        {
            if (expression is null)  return await _dbContext.Set<T>().CountAsync();
            else return await _dbContext.Set<T>().CountAsync(expression);
        }

        public async Task<T> GetByExpressionSingleAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[] includes = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.Where(expression).FirstOrDefaultAsync();
        }

        #endregion
    }
}
