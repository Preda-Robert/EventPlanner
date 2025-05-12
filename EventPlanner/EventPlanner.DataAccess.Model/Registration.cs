using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Registration
  {
    [Key]
    public int RegistrationId { get; set; }
    [Required]
    public int UserId { get; set; }

    public ApplicationUser User { get; set; }

    public int EventId { get; set; }
    public virtual Event Event { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool IsConfirmed { get; set; }
  }

}
