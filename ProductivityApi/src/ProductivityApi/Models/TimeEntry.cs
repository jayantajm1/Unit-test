using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductivityApi.Models;

public class TimeEntry
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int TaskId { get; set; }

    // Navigation property
    [ForeignKey("TaskId")]
    public virtual ProductivityTask Task { get; set; } = null!;
}
