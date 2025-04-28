using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models;
public class Event
{
  [Key]

  public int EventId { get; set; }

  public string Title { get; set; }

  public string Description { get; set; }

  public DateTime Date { get; set; }

  public string Location { get; set; }
  [ForeignKey("Host")]
  public int HostId { get; set; }
  public virtual Host? Host { get; set; }

  public virtual ICollection<EventGuest>? EventGuests { get; set; }
  public virtual ICollection<Registration>? Registrations { get; set; }

  public virtual ICollection<Comment>? Comments { get; set; }
}
