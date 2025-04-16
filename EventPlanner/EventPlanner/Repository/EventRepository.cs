using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
  public class EventRepository : Repository<Event>, IEventRepository
  {
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Event>> GetEventsWithGuestsAsync()
    {
      return await _context.Events
          .Include(e => e.Guests)
          .Include(e => e.Comments)
          .Include(e => e.Host)
          .ToListAsync();
    }
  }

}
