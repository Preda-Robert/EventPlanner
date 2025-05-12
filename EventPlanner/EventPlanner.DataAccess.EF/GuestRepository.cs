using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class GuestRepository : Repository<Guest>, IGuestRepository
  {
    private readonly ApplicationDbContext _context;
    public GuestRepository(ApplicationDbContext context) : base(context)
    {
      _context = context;
    }
    // Add any additional methods specific to Guest repository here
  }

}
