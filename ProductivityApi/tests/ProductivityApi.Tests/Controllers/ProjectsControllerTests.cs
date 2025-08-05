using Microsoft.AspNetCore.Mvc.Testing;
using ProductivityApi.Models.DTOs;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ProductivityApi.Tests.Controllers;

public class ProjectsControllerTests : IClassFixture<ProductivityApiFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProjectsControllerTests(ProductivityApiFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProjects_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetProjects_ReturnsProjectList()
    {
        // Act
        var response = await _client.GetAsync("/api/projects");
        var content = await response.Content.ReadAsStringAsync();
        var projects = JsonSerializer.Deserialize<List<ProjectDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(projects);
        Assert.Equal(2, projects.Count);
        Assert.Contains(projects, p => p.Name == "Test Project 1");
        Assert.Contains(projects, p => p.Name == "Test Project 2");
    }

    [Fact]
    public async Task GetProject_WithValidId_ReturnsProject()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/1");
        var content = await response.Content.ReadAsStringAsync();
        var project = JsonSerializer.Deserialize<ProjectDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(project);
        Assert.Equal("Test Project 1", project.Name);
        Assert.Equal("Test Description 1", project.Description);
    }

    [Fact]
    public async Task GetProject_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newProject = new CreateProjectDto
        {
            Name = "New Test Project",
            Description = "New Test Description",
            Status = "Planning",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30)
        };

        // Act
        var response = await _client.PostAsync("/api/projects", newProject.ToJsonContent());
        var content = await response.Content.ReadAsStringAsync();
        var createdProject = JsonSerializer.Deserialize<ProjectDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdProject);
        Assert.Equal("New Test Project", createdProject.Name);
        Assert.Equal("New Test Description", createdProject.Description);
    }

    [Fact]
    public async Task UpdateProject_WithValidData_ReturnsUpdatedProject()
    {
        // Arrange
        var updateProject = new UpdateProjectDto
        {
            Name = "Updated Project Name",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsync("/api/projects/1", updateProject.ToJsonContent());
        var content = await response.Content.ReadAsStringAsync();
        var updatedProject = JsonSerializer.Deserialize<ProjectDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(updatedProject);
        Assert.Equal("Updated Project Name", updatedProject.Name);
        Assert.Equal("Updated Description", updatedProject.Description);
    }

    [Fact]
    public async Task UpdateProject_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateProject = new UpdateProjectDto
        {
            Name = "Updated Project Name"
        };

        // Act
        var response = await _client.PutAsync("/api/projects/999", updateProject.ToJsonContent());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_WithValidId_ReturnsNoContent()
    {
        // First create a project to delete
        var newProject = new CreateProjectDto
        {
            Name = "Project to Delete",
            Description = "This will be deleted",
            Status = "Planning",
            StartDate = DateTime.Now
        };

        var createResponse = await _client.PostAsync("/api/projects", newProject.ToJsonContent());
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdProject = JsonSerializer.Deserialize<ProjectDto>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.DeleteAsync($"/api/projects/{createdProject!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the project is deleted
        var getResponse = await _client.GetAsync($"/api/projects/{createdProject.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/projects/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectsByStatus_ReturnsFilteredProjects()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/status/InProgress");
        var content = await response.Content.ReadAsStringAsync();
        var projects = JsonSerializer.Deserialize<List<ProjectDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(projects);
        Assert.Single(projects);
        Assert.Equal("Test Project 1", projects[0].Name);
        Assert.Equal("InProgress", projects[0].Status);
    }
}
