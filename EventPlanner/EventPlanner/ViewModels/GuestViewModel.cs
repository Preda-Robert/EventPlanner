using EventPlanner.Enums;

namespace EventPlanner.ViewModels
{
  public class GuestViewModel
  {
    public int GuestId { get; set; }

    public int? EventId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public GuestType Type { get; set; }
  }
}
