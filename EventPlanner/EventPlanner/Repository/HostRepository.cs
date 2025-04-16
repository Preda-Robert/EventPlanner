using EventPlanner.Models;

using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class HostRepository : Repository<Models.Host>, IHostRepository
  {
    private readonly AppDbContext _context;
    public HostRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }
    // Add any additional methods specific to Host repository here
  }

}
