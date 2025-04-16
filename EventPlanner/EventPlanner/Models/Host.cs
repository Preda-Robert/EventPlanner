using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Host
  {
    public int HostId { get; set; }

    [Required]
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public ICollection<Event>? Events { get; set; }
  }

}
