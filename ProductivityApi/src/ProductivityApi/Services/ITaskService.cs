using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public interface ITaskService
{
    Task<IEnumerable<ProductivityTaskDto>> GetAllTasksAsync();
    Task<ProductivityTaskDto?> GetTaskByIdAsync(int id);
    Task<ProductivityTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<ProductivityTaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int id);
    Task<IEnumerable<ProductivityTaskDto>> GetTasksByProjectIdAsync(int projectId);
    Task<IEnumerable<ProductivityTaskDto>> GetTasksByStatusAsync(ProductivityTaskStatus status);
    Task<IEnumerable<ProductivityTaskDto>> GetTasksByPriorityAsync(TaskPriority priority);
    Task<IEnumerable<ProductivityTaskDto>> GetOverdueTasksAsync();
}
