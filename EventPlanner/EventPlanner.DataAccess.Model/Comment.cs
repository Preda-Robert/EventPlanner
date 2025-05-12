using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
  public class Comment
  {
    [Key]
    public int CommentId { get; set; }
    [ForeignKey("Event")]

    public int EventId { get; set; }
    public virtual Event Event { get; set; }
    [ForeignKey("User")]

    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }

}
