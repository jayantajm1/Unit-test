using Microsoft.EntityFrameworkCore;
using ProductivityApi.Models;
using ProductivityApi.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ProductivityContext>(options =>
    options.UseInMemoryDatabase("ProductivityDb"));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITimeTrackingService, TimeTrackingService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductivityContext>();
    SeedData(context);
}

app.Run();

static void SeedData(ProductivityContext context)
{
    if (!context.Projects.Any())
    {
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Website Redesign", Description = "Complete redesign of company website", Status = ProjectStatus.InProgress, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(30) },
            new Project { Id = 2, Name = "Mobile App", Description = "Development of mobile application", Status = ProjectStatus.Planning, StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddDays(90) },
            new Project { Id = 3, Name = "Database Migration", Description = "Migrate to new database system", Status = ProjectStatus.Completed, StartDate = DateTime.Now.AddDays(-60), EndDate = DateTime.Now.AddDays(-10) }
        };

        context.Projects.AddRange(projects);
        context.SaveChanges();

        var tasks = new List<ProductivityTask>
        {
            new ProductivityTask { Id = 1, Title = "Design Homepage", Description = "Create new homepage design", Status = ProductivityTaskStatus.InProgress, Priority = TaskPriority.High, ProjectId = 1, DueDate = DateTime.Now.AddDays(7) },
            new ProductivityTask { Id = 2, Title = "Setup Database", Description = "Configure database for new project", Status = ProductivityTaskStatus.Completed, Priority = TaskPriority.Medium, ProjectId = 1, DueDate = DateTime.Now.AddDays(-5) },
            new ProductivityTask { Id = 3, Title = "Create Wireframes", Description = "Design app wireframes", Status = ProductivityTaskStatus.ToDo, Priority = TaskPriority.High, ProjectId = 2, DueDate = DateTime.Now.AddDays(14) },
            new ProductivityTask { Id = 4, Title = "Data Backup", Description = "Backup existing data", Status = ProductivityTaskStatus.Completed, Priority = TaskPriority.Critical, ProjectId = 3, DueDate = DateTime.Now.AddDays(-15) }
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntry { Id = 1, TaskId = 1, Description = "Working on homepage layout", StartTime = DateTime.Now.AddHours(-3), EndTime = DateTime.Now.AddHours(-1), Duration = TimeSpan.FromHours(2) },
            new TimeEntry { Id = 2, TaskId = 2, Description = "Database configuration", StartTime = DateTime.Now.AddDays(-1).AddHours(-4), EndTime = DateTime.Now.AddDays(-1).AddHours(-1), Duration = TimeSpan.FromHours(3) },
            new TimeEntry { Id = 3, TaskId = 1, Description = "Homepage styling", StartTime = DateTime.Now.AddDays(-2).AddHours(-2), EndTime = DateTime.Now.AddDays(-2), Duration = TimeSpan.FromHours(2) }
        };

        context.TimeEntries.AddRange(timeEntries);
        context.SaveChanges();
    }
}

// Make Program class public for testing
public partial class Program { }
