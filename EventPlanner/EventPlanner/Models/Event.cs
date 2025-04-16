using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Event
  {
    public int EventId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    // Navigation properties
    public int HostId { get; set; }
    public Host? Host { get; set; }

    public ICollection<Guest>? Guests { get; set; }

    public ICollection<Comment>? Comments { get; set; }
  }

}
