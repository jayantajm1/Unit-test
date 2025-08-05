using Microsoft.AspNetCore.Mvc.Testing;
using ProductivityApi.Models.DTOs;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ProductivityApi.Tests.Controllers;

public class TasksControllerTests : IClassFixture<ProductivityApiFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksControllerTests(ProductivityApiFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetTasks_ReturnsTaskList()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<ProductivityTaskDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Count);
        Assert.Contains(tasks, t => t.Title == "Test Task 1");
        Assert.Contains(tasks, t => t.Title == "Test Task 2");
    }

    [Fact]
    public async Task GetTask_WithValidId_ReturnsTask()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/1");
        var content = await response.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<ProductivityTaskDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(task);
        Assert.Equal("Test Task 1", task.Title);
        Assert.Equal("Test Task Description 1", task.Description);
    }

    [Fact]
    public async Task GetTask_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newTask = new CreateTaskDto
        {
            Title = "New Test Task",
            Description = "New Test Description",
            Status = "ToDo",
            Priority = "Medium",
            ProjectId = 1,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var response = await _client.PostAsync("/api/tasks", newTask.ToJsonContent());
        var content = await response.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<ProductivityTaskDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdTask);
        Assert.Equal("New Test Task", createdTask.Title);
        Assert.Equal("New Test Description", createdTask.Description);
        Assert.Equal("ToDo", createdTask.Status);
        Assert.Equal("Medium", createdTask.Priority);
    }

    [Fact]
    public async Task UpdateTask_WithValidData_ReturnsUpdatedTask()
    {
        // Arrange
        var updateTask = new UpdateTaskDto
        {
            Title = "Updated Task Title",
            Description = "Updated Description",
            Status = "Completed"
        };

        // Act
        var response = await _client.PutAsync("/api/tasks/1", updateTask.ToJsonContent());
        var content = await response.Content.ReadAsStringAsync();
        var updatedTask = JsonSerializer.Deserialize<ProductivityTaskDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated Task Title", updatedTask.Title);
        Assert.Equal("Updated Description", updatedTask.Description);
        Assert.Equal("Completed", updatedTask.Status);
    }

    [Fact]
    public async Task DeleteTask_WithValidId_ReturnsNoContent()
    {
        // First create a task to delete
        var newTask = new CreateTaskDto
        {
            Title = "Task to Delete",
            Description = "This will be deleted",
            Status = "ToDo",
            Priority = "Low",
            ProjectId = 1
        };

        var createResponse = await _client.PostAsync("/api/tasks", newTask.ToJsonContent());
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdTask = JsonSerializer.Deserialize<ProductivityTaskDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{createdTask!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the task is deleted
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetTasksByProject_ReturnsFilteredTasks()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/project/1");
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<ProductivityTaskDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Count);
        Assert.All(tasks, t => Assert.Equal(1, t.ProjectId));
    }

    [Fact]
    public async Task GetTasksByStatus_ReturnsFilteredTasks()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/status/InProgress");
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<ProductivityTaskDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Test Task 1", tasks[0].Title);
        Assert.Equal("InProgress", tasks[0].Status);
    }

    [Fact]
    public async Task GetTasksByPriority_ReturnsFilteredTasks()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/priority/High");
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<ProductivityTaskDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Test Task 1", tasks[0].Title);
        Assert.Equal("High", tasks[0].Priority);
    }

    [Fact]
    public async Task GetOverdueTasks_ReturnsOverdueTasks()
    {
        // First create an overdue task
        var overdueTask = new CreateTaskDto
        {
            Title = "Overdue Task",
            Description = "This task is overdue",
            Status = "InProgress",
            Priority = "High",
            ProjectId = 1,
            DueDate = DateTime.Now.AddDays(-1) // Past due date
        };

        await _client.PostAsync("/api/tasks", overdueTask.ToJsonContent());

        // Act
        var response = await _client.GetAsync("/api/tasks/overdue");
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<ProductivityTaskDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(tasks);
        Assert.Contains(tasks, t => t.Title == "Overdue Task");
    }
}
