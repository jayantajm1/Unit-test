namespace ProductivityApi.Models.DTOs;

public class TimeEntryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
}

public class CreateTimeEntryDto
{
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int TaskId { get; set; }
}

public class UpdateTimeEntryDto
{
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? TaskId { get; set; }
}
