using System.Linq.Expressions;

namespace EventPlanner.Repository.Interfaces
{
  public interface IRepository<T> where T : class
  {
    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> GetAllAsync(
     Expression<Func<T, bool>>? filter = null,
     Func<IQueryable<T>, IQueryable<T>>? include = null);

    Task<T> GetByIdAsync(object id);
    Task<T?> GetByIdAsync(
    Expression<Func<T, bool>> predicate,
    Func<IQueryable<T>, IQueryable<T>>? include = null);

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();
  }

}
