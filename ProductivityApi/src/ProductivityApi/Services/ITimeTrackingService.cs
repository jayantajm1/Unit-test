using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public interface ITimeTrackingService
{
    Task<IEnumerable<TimeEntryDto>> GetAllTimeEntriesAsync();
    Task<TimeEntryDto?> GetTimeEntryByIdAsync(int id);
    Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryDto createTimeEntryDto);
    Task<TimeEntryDto?> UpdateTimeEntryAsync(int id, UpdateTimeEntryDto updateTimeEntryDto);
    Task<bool> DeleteTimeEntryAsync(int id);
    Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByTaskIdAsync(int taskId);
    Task<IEnumerable<TimeEntryDto>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<TimeSpan> GetTotalTimeForTaskAsync(int taskId);
    Task<TimeSpan> GetTotalTimeForProjectAsync(int projectId);
    Task<TimeEntryDto?> StartTimerAsync(int taskId, string description);
    Task<TimeEntryDto?> StopTimerAsync(int timeEntryId);
}
