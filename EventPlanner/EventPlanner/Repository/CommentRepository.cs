using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;

namespace EventPlanner.Repository
{
  public class CommentRepository : Repository<Comment>, ICommentRepository
  {
    private readonly AppDbContext _context;
    public CommentRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }
    // Add any additional methods specific to Comment repository here
  }

}
