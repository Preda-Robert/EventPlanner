using EventPlanner.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.ViewModels
{
  public class GuestEditViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Role { get; set; }

    [Required]
    public GuestType Type { get; set; }

    public List<SelectListItem>? GuestTypes { get; set; }
  }
}
