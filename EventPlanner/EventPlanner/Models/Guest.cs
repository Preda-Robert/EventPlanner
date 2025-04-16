using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Guest
  {
    public int GuestId { get; set; }

    [Required]
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    // RSVP status (e.g. "Yes", "No", "Maybe")
    public string RSVP { get; set; }

    // Navigation
    public int EventId { get; set; }
    public Event Event { get; set; }
  }

}
