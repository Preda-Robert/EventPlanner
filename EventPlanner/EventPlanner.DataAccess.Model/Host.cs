using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Host
  {
    [Key]

    public int HostId { get; set; }

    public string Name { get; set; }

    public string ContactInfo { get; set; }

    public virtual ICollection<Event>? EventsHosted { get; set; }
  }

}
