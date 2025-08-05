using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public class TaskService : ITaskService
{
    private readonly ProductivityContext _context;
    private readonly IMapper _mapper;

    public TaskService(ProductivityContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductivityTaskDto>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductivityTaskDto>>(tasks);
    }

    public async Task<ProductivityTaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task != null ? _mapper.Map<ProductivityTaskDto>(task) : null;
    }

    public async Task<ProductivityTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        var task = _mapper.Map<ProductivityTask>(createTaskDto);

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(task)
            .Reference(t => t.Project)
            .LoadAsync();

        return _mapper.Map<ProductivityTaskDto>(task);
    }

    public async Task<ProductivityTaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return null;

        _mapper.Map(updateTaskDto, task);
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductivityTaskDto>(task);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ProductivityTaskDto>> GetTasksByProjectIdAsync(int projectId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductivityTaskDto>>(tasks);
    }

    public async Task<IEnumerable<ProductivityTaskDto>> GetTasksByStatusAsync(ProductivityTaskStatus status)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Status == status)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductivityTaskDto>>(tasks);
    }

    public async Task<IEnumerable<ProductivityTaskDto>> GetTasksByPriorityAsync(TaskPriority priority)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.Priority == priority)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductivityTaskDto>>(tasks);
    }

    public async Task<IEnumerable<ProductivityTaskDto>> GetOverdueTasksAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TimeEntries)
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != ProductivityTaskStatus.Completed)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductivityTaskDto>>(tasks);
    }
}
