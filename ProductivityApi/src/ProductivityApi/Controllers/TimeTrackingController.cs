using Microsoft.AspNetCore.Mvc;
using ProductivityApi.Models.DTOs;
using ProductivityApi.Services;

namespace ProductivityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeTrackingController : ControllerBase
{
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeTrackingController(ITimeTrackingService timeTrackingService)
    {
        _timeTrackingService = timeTrackingService;
    }

    /// <summary>
    /// Get all time entries
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntries()
    {
        var timeEntries = await _timeTrackingService.GetAllTimeEntriesAsync();
        return Ok(timeEntries);
    }

    /// <summary>
    /// Get a specific time entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TimeEntryDto>> GetTimeEntry(int id)
    {
        var timeEntry = await _timeTrackingService.GetTimeEntryByIdAsync(id);

        if (timeEntry == null)
        {
            return NotFound($"Time entry with ID {id} not found.");
        }

        return Ok(timeEntry);
    }

    /// <summary>
    /// Create a new time entry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TimeEntryDto>> CreateTimeEntry(CreateTimeEntryDto createTimeEntryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var timeEntry = await _timeTrackingService.CreateTimeEntryAsync(createTimeEntryDto);
        return CreatedAtAction(nameof(GetTimeEntry), new { id = timeEntry.Id }, timeEntry);
    }

    /// <summary>
    /// Update an existing time entry
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TimeEntryDto>> UpdateTimeEntry(int id, UpdateTimeEntryDto updateTimeEntryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var timeEntry = await _timeTrackingService.UpdateTimeEntryAsync(id, updateTimeEntryDto);

        if (timeEntry == null)
        {
            return NotFound($"Time entry with ID {id} not found.");
        }

        return Ok(timeEntry);
    }

    /// <summary>
    /// Delete a time entry
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimeEntry(int id)
    {
        var result = await _timeTrackingService.DeleteTimeEntryAsync(id);

        if (!result)
        {
            return NotFound($"Time entry with ID {id} not found.");
        }

        return NoContent();
    }

    /// <summary>
    /// Get time entries by task ID
    /// </summary>
    [HttpGet("task/{taskId}")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByTask(int taskId)
    {
        var timeEntries = await _timeTrackingService.GetTimeEntriesByTaskIdAsync(taskId);
        return Ok(timeEntries);
    }

    /// <summary>
    /// Get time entries by date range
    /// </summary>
    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var timeEntries = await _timeTrackingService.GetTimeEntriesByDateRangeAsync(startDate, endDate);
        return Ok(timeEntries);
    }

    /// <summary>
    /// Get total time for a specific task
    /// </summary>
    [HttpGet("task/{taskId}/total")]
    public async Task<ActionResult<object>> GetTotalTimeForTask(int taskId)
    {
        var totalTime = await _timeTrackingService.GetTotalTimeForTaskAsync(taskId);
        return Ok(new { TaskId = taskId, TotalTime = totalTime.ToString() });
    }

    /// <summary>
    /// Get total time for a specific project
    /// </summary>
    [HttpGet("project/{projectId}/total")]
    public async Task<ActionResult<object>> GetTotalTimeForProject(int projectId)
    {
        var totalTime = await _timeTrackingService.GetTotalTimeForProjectAsync(projectId);
        return Ok(new { ProjectId = projectId, TotalTime = totalTime.ToString() });
    }

    /// <summary>
    /// Start a timer for a task
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<TimeEntryDto>> StartTimer([FromBody] StartTimerDto startTimerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var timeEntry = await _timeTrackingService.StartTimerAsync(startTimerDto.TaskId, startTimerDto.Description);

        if (timeEntry == null)
        {
            return NotFound($"Task with ID {startTimerDto.TaskId} not found.");
        }

        return Ok(timeEntry);
    }

    /// <summary>
    /// Stop a running timer
    /// </summary>
    [HttpPost("stop/{timeEntryId}")]
    public async Task<ActionResult<TimeEntryDto>> StopTimer(int timeEntryId)
    {
        var timeEntry = await _timeTrackingService.StopTimerAsync(timeEntryId);

        if (timeEntry == null)
        {
            return NotFound($"Time entry with ID {timeEntryId} not found or already stopped.");
        }

        return Ok(timeEntry);
    }
}

public class StartTimerDto
{
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
}
