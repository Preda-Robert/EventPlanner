using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EventPlanner.Repository
{
  public class Repository<T> : IRepository<T> where T : class
  {
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
      _context = context;
      _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<T> GetByIdAsync(object id) => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public Task SaveAsync() => _context.SaveChangesAsync();

    public async Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>>? filter = null,
    Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
      IQueryable<T> query = _context.Set<T>();

      if (filter != null)
        query = query.Where(filter);

      if (include != null)
        query = include(query);

      return await query.ToListAsync();
    }
    public async Task<T?> GetByIdAsync(
    Expression<Func<T, bool>> predicate,
    Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
      IQueryable<T> query = _context.Set<T>();

      if (include != null)
        query = include(query);

      return await query.FirstOrDefaultAsync(predicate);
    }

    public IQueryable<T> GetQueryable()
    {
      return _dbSet.AsQueryable();
    }

  }


}
