using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;




namespace EventPlanner.Models
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Host> Hosts { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<EventGuest> EventGuests { get; set; }
    public DbSet<Comment> Comments { get; set; }




  }

}
