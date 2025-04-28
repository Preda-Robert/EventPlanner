using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
  [PrimaryKey(nameof(EventId), nameof(GuestId))]
  public class EventGuest
  {

    [ForeignKey("Event")]
    public int EventId { get; set; }
    public virtual Event Event { get; set; }
    [ForeignKey("Guest")]
    public int GuestId { get; set; }
    public virtual Guest Guest { get; set; }
  }

}
