using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;
using ProductivityApi.Models.Mappings;
using ProductivityApi.Services;
using Xunit;

namespace ProductivityApi.Tests.Services;

public class ProjectServiceTests
{
    private readonly ProductivityContext _context;
    private readonly IMapper _mapper;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ProductivityContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ProductivityContext(options);

        // Setup AutoMapper
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();

        _service = new ProjectService(_context, _mapper);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                Name = "Test Project 1",
                Description = "Test Description 1",
                Status = ProjectStatus.InProgress,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(10)
            },
            new Project
            {
                Id = 2,
                Name = "Test Project 2",
                Description = "Test Description 2",
                Status = ProjectStatus.Planning,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(25)
            }
        };

        _context.Projects.AddRange(projects);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllProjectsAsync_ReturnsAllProjects()
    {
        // Act
        var result = await _service.GetAllProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Test Project 1");
        Assert.Contains(result, p => p.Name == "Test Project 2");
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithValidId_ReturnsProject()
    {
        // Act
        var result = await _service.GetProjectByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Project 1", result.Name);
        Assert.Equal("Test Description 1", result.Description);
        Assert.Equal("InProgress", result.Status);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _service.GetProjectByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProjectAsync_WithValidData_CreatesProject()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Name = "New Project",
            Description = "New Description",
            Status = "Planning",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30)
        };

        // Act
        var result = await _service.CreateProjectAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Project", result.Name);
        Assert.Equal("New Description", result.Description);
        Assert.Equal("Planning", result.Status);

        // Verify it was saved to database
        var savedProject = await _context.Projects.FindAsync(result.Id);
        Assert.NotNull(savedProject);
        Assert.Equal("New Project", savedProject.Name);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithValidData_UpdatesProject()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Status = "Completed"
        };

        // Act
        var result = await _service.UpdateProjectAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal("Completed", result.Status);

        // Verify it was updated in database
        var updatedProject = await _context.Projects.FindAsync(1);
        Assert.NotNull(updatedProject);
        Assert.Equal("Updated Name", updatedProject.Name);
        Assert.Equal("Updated Description", updatedProject.Description);
        Assert.Equal(ProjectStatus.Completed, updatedProject.Status);
    }

    [Fact]
    public async Task UpdateProjectAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Name = "Updated Name"
        };

        // Act
        var result = await _service.UpdateProjectAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_WithValidId_DeletesProject()
    {
        // Act
        var result = await _service.DeleteProjectAsync(1);

        // Assert
        Assert.True(result);

        // Verify it was deleted from database
        var deletedProject = await _context.Projects.FindAsync(1);
        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteProjectAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteProjectAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetProjectsByStatusAsync_ReturnsFilteredProjects()
    {
        // Act
        var result = await _service.GetProjectsByStatusAsync(ProjectStatus.InProgress);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Project 1", result.First().Name);
        Assert.Equal("InProgress", result.First().Status);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
