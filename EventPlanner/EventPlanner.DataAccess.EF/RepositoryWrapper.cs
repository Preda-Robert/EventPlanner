using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class RepositoryWrapper : IRepositoryWrapper
  {
    private readonly ApplicationDbContext _context;
    public IRepository<Comment> Comment { get; private set; }
    public IRepository<Event> Event { get; private set; }
    public IRepository<EventGuest> EventGuest { get; private set; }
    public IRepository<Guest> Guest { get; private set; }
    public IRepository<Models.Host> Host { get; private set; }
    public IRepository<Registration> Registration { get; private set; }

    public RepositoryWrapper(ApplicationDbContext context)
    {
      _context = context;
      Comment = new Repository<Comment>(_context);
      Event = new Repository<Event>(_context);
      EventGuest = new Repository<EventGuest>(_context);
      Guest = new Repository<Guest>(_context);
      Host = new Repository<Models.Host>(_context);
      Registration = new Repository<Registration>(_context);
    }

    public Task SaveAsync() => _context.SaveChangesAsync();
  }


}
