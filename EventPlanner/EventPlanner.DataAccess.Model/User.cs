using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models
{
  public class ApplicationUser : IdentityUser<int>  // Primary key type: int
  {
    public string Name { get; set; }

    // Navigation
    public virtual ICollection<Registration> Registrations { get; set; }
  }
}
