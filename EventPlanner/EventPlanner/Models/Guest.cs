using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{

  public class Guest
  {
    [Key]

    public int GuestId { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public int Type { get; set; }

    public virtual ICollection<EventGuest> EventGuests { get; set; }
  }

}
