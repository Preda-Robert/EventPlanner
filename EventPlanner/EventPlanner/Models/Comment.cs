using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
  public class Comment
  {
    public int CommentId { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string AuthorName { get; set; }

    // Navigation
    public int EventId { get; set; }
    public Event Event { get; set; }
  }

}
