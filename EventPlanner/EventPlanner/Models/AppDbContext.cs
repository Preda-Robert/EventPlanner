using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Models
{
  public class AppDbContext : DbContext
  {
    public AppDbContext()
    {
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Host> Hosts { get; set; }
    public DbSet<Comment> Comments { get; set; }



  }
}
