using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductivityApi.Models;
using System.Text;
using System.Text.Json;

namespace ProductivityApi.Tests;

public class ProductivityApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ProductivityContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database context with unique database name
            services.AddDbContext<ProductivityContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ProductivityContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            // Seed the database with test data
            SeedTestData(db);
        });
    }

    private void SeedTestData(ProductivityContext context)
    {
        // Clear existing data
        context.TimeEntries.RemoveRange(context.TimeEntries);
        context.Tasks.RemoveRange(context.Tasks);
        context.Projects.RemoveRange(context.Projects);
        context.SaveChanges();

        // Add test projects
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

        context.Projects.AddRange(projects);
        context.SaveChanges();

        // Add test tasks
        var tasks = new List<ProductivityTask>
        {
            new ProductivityTask
            {
                Id = 1,
                Title = "Test Task 1",
                Description = "Test Task Description 1",
                Status = ProductivityTaskStatus.InProgress,
                Priority = TaskPriority.High,
                ProjectId = 1,
                DueDate = DateTime.Now.AddDays(5)
            },
            new ProductivityTask
            {
                Id = 2,
                Title = "Test Task 2",
                Description = "Test Task Description 2",
                Status = ProductivityTaskStatus.ToDo,
                Priority = TaskPriority.Medium,
                ProjectId = 1,
                DueDate = DateTime.Now.AddDays(10)
            }
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();

        // Add test time entries
        var timeEntries = new List<TimeEntry>
        {
            new TimeEntry
            {
                Id = 1,
                TaskId = 1,
                Description = "Test Time Entry 1",
                StartTime = DateTime.Now.AddHours(-2),
                EndTime = DateTime.Now.AddHours(-1),
                Duration = TimeSpan.FromHours(1)
            }
        };

        context.TimeEntries.AddRange(timeEntries);
        context.SaveChanges();
    }
}

public static class HttpContentExtensions
{
    public static StringContent ToJsonContent(this object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
