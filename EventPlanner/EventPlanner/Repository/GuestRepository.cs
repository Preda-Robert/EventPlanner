using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class GuestRepository : Repository<Guest>, IGuestRepository
  {
    private readonly AppDbContext _context;
    public GuestRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }
    // Add any additional methods specific to Guest repository here
  }

}
