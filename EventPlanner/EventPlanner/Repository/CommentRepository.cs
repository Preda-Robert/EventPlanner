using EventPlanner.Models;

using EventPlanner.Repository.Interfaces;
namespace EventPlanner.Repository
{
  public class CommentRepository : Repository<Comment>, ICommentRepository
  {
    private readonly ApplicationDbContext _context;
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
      _context = context;
    }
  }

}
