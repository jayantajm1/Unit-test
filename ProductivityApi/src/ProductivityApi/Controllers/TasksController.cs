using Microsoft.AspNetCore.Mvc;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;
using ProductivityApi.Services;

namespace ProductivityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductivityTaskDto>>> GetTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductivityTaskDto>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);

        if (task == null)
        {
            return NotFound($"Task with ID {id} not found.");
        }

        return Ok(task);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductivityTaskDto>> CreateTask(CreateTaskDto createTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var task = await _taskService.CreateTaskAsync(createTaskDto);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductivityTaskDto>> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);

        if (task == null)
        {
            return NotFound($"Task with ID {id} not found.");
        }

        return Ok(task);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);

        if (!result)
        {
            return NotFound($"Task with ID {id} not found.");
        }

        return NoContent();
    }

    /// <summary>
    /// Get tasks by project ID
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProductivityTaskDto>>> GetTasksByProject(int projectId)
    {
        var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
        return Ok(tasks);
    }

    /// <summary>
    /// Get tasks by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<ProductivityTaskDto>>> GetTasksByStatus(ProductivityTaskStatus status)
    {
        var tasks = await _taskService.GetTasksByStatusAsync(status);
        return Ok(tasks);
    }

    /// <summary>
    /// Get tasks by priority
    /// </summary>
    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<IEnumerable<ProductivityTaskDto>>> GetTasksByPriority(TaskPriority priority)
    {
        var tasks = await _taskService.GetTasksByPriorityAsync(priority);
        return Ok(tasks);
    }

    /// <summary>
    /// Get overdue tasks
    /// </summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<ProductivityTaskDto>>> GetOverdueTasks()
    {
        var tasks = await _taskService.GetOverdueTasksAsync();
        return Ok(tasks);
    }
}
