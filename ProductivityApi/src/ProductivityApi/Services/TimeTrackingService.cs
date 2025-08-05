using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public class TimeTrackingService : ITimeTrackingService
{
    private readonly ProductivityContext _context;
    private readonly IMapper _mapper;

    public TimeTrackingService(ProductivityContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeEntryDto>> GetAllTimeEntriesAsync()
    {
        var timeEntries = await _context.TimeEntries
            .Include(te => te.Task)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }

    public async Task<TimeEntryDto?> GetTimeEntryByIdAsync(int id)
    {
        var timeEntry = await _context.TimeEntries
            .Include(te => te.Task)
            .FirstOrDefaultAsync(te => te.Id == id);

        return timeEntry != null ? _mapper.Map<TimeEntryDto>(timeEntry) : null;
    }

    public async Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryDto createTimeEntryDto)
    {
        var timeEntry = _mapper.Map<TimeEntry>(createTimeEntryDto);

        // Calculate duration if end time is provided
        if (timeEntry.EndTime.HasValue)
        {
            timeEntry.Duration = timeEntry.EndTime.Value - timeEntry.StartTime;
        }

        _context.TimeEntries.Add(timeEntry);
        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(timeEntry)
            .Reference(te => te.Task)
            .LoadAsync();

        return _mapper.Map<TimeEntryDto>(timeEntry);
    }

    public async Task<TimeEntryDto?> UpdateTimeEntryAsync(int id, UpdateTimeEntryDto updateTimeEntryDto)
    {
        var timeEntry = await _context.TimeEntries
            .Include(te => te.Task)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (timeEntry == null)
            return null;

        _mapper.Map(updateTimeEntryDto, timeEntry);

        // Recalculate duration if times are updated
        if (timeEntry.EndTime.HasValue)
        {
            timeEntry.Duration = timeEntry.EndTime.Value - timeEntry.StartTime;
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<TimeEntryDto>(timeEntry);
    }

    public async Task<bool> DeleteTimeEntryAsync(int id)
    {
        var timeEntry = await _context.TimeEntries.FindAsync(id);
        if (timeEntry == null)
            return false;

        _context.TimeEntries.Remove(timeEntry);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByTaskIdAsync(int taskId)
    {
        var timeEntries = await _context.TimeEntries
            .Include(te => te.Task)
            .Where(te => te.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }

    public async Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var timeEntries = await _context.TimeEntries
            .Include(te => te.Task)
            .Where(te => te.StartTime >= startDate && te.StartTime <= endDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TimeEntryDto>>(timeEntries);
    }

    public async Task<TimeSpan> GetTotalTimeForTaskAsync(int taskId)
    {
        var totalTicks = await _context.TimeEntries
            .Where(te => te.TaskId == taskId && te.EndTime.HasValue)
            .SumAsync(te => te.Duration.Ticks);

        return new TimeSpan(totalTicks);
    }

    public async Task<TimeSpan> GetTotalTimeForProjectAsync(int projectId)
    {
        var totalTicks = await _context.TimeEntries
            .Include(te => te.Task)
            .Where(te => te.Task.ProjectId == projectId && te.EndTime.HasValue)
            .SumAsync(te => te.Duration.Ticks);

        return new TimeSpan(totalTicks);
    }

    public async Task<TimeEntryDto?> StartTimerAsync(int taskId, string description)
    {
        // Check if task exists
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
            return null;

        var timeEntry = new TimeEntry
        {
            TaskId = taskId,
            Description = description,
            StartTime = DateTime.Now,
            CreatedAt = DateTime.UtcNow
        };

        _context.TimeEntries.Add(timeEntry);
        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(timeEntry)
            .Reference(te => te.Task)
            .LoadAsync();

        return _mapper.Map<TimeEntryDto>(timeEntry);
    }

    public async Task<TimeEntryDto?> StopTimerAsync(int timeEntryId)
    {
        var timeEntry = await _context.TimeEntries
            .Include(te => te.Task)
            .FirstOrDefaultAsync(te => te.Id == timeEntryId);

        if (timeEntry == null || timeEntry.EndTime.HasValue)
            return null;

        timeEntry.EndTime = DateTime.Now;
        timeEntry.Duration = timeEntry.EndTime.Value - timeEntry.StartTime;

        await _context.SaveChangesAsync();

        return _mapper.Map<TimeEntryDto>(timeEntry);
    }
}
