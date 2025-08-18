using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Company.Project.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllFilteredAsync(string[] filters);
        Task DeleteRangeAsync(ICollection<T> entities);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(int Skip, int Take);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>>[] includes = null);
        Task<IEnumerable<T>> GetByNameAsync(Expression<Func<T, bool>> expression, string name);
        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);

        Task<T> GetByExpressionSingleAsync(Expression<Func<T, bool>> expression,
            Expression<Func<T, object>>[] includes = null);

        Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression,
            Expression<Func<T, object>>[] includes = null);

        Task<IEnumerable<T>> GetByExpressionAsync(int Skip, int Take, Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>>? expression = default);
    }
}