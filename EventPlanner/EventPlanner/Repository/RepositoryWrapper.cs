using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class RepositoryWrapper : IRepositoryWrapper
  {
    private readonly AppDbContext _context;

    private IEventRepository _event;
    private IRepository<Guest> _guest;
    private IRepository<Models.Host> _host;
    private IRepository<Comment> _comment;

    public RepositoryWrapper(AppDbContext context)
    {
      _context = context;
    }

    public IEventRepository Event => _event ??= new EventRepository(_context);
    public IRepository<Guest> Guest => _guest ??= new Repository<Guest>(_context);
    public IRepository<Models.Host> Host => _host ??= new Repository<Models.Host>(_context);
    public IRepository<Comment> Comment => _comment ??= new Repository<Comment>(_context);

    public async Task SaveAsync()
    {
      await _context.SaveChangesAsync();
    }
  }

}
