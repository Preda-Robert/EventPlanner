using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class HostRepository : Repository<EventPlanner.Models.Host>, IHostRepository
  {
    private readonly ApplicationDbContext _context;
    public HostRepository(ApplicationDbContext context) : base(context)
    {
      _context = context;
    }
  }

}
