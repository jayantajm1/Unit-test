# ProductivityApi

A comprehensive ASP.NET Core Web API for productivity and project management, featuring project tracking, task management, and time tracking capabilities.

## 🚀 Features

- **Project Management**: Create, update, and manage projects with status tracking
- **Task Management**: Organize tasks within projects with priority levels and due dates
- **Time Tracking**: Track time spent on tasks with start/stop timer functionality
- **RESTful API**: Clean and intuitive API endpoints
- **Entity Framework Core**: Database operations with in-memory and SQL Server support
- **AutoMapper**: Object mapping for DTOs
- **Comprehensive Testing**: Unit and integration tests
- **Docker Support**: Containerized deployment
- **Swagger Documentation**: Interactive API documentation

## 🏗️ Architecture

The solution follows Clean Architecture principles:

```
ProductivityApi/
├── src/
│   └── ProductivityApi/
│       ├── Controllers/          # API Controllers
│       ├── Models/               # Domain models and DTOs
│       ├── Services/             # Business logic services
│       └── Program.cs            # Application entry point
├── tests/
│   └── ProductivityApi.Tests/
│       ├── Controllers/          # Controller tests
│       └── Services/             # Service tests
├── Dockerfile                    # Docker configuration
├── docker-compose.yml           # Multi-container setup
└── .dockerignore                # Docker ignore file
```

## 📋 API Endpoints

### Projects
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project
- `GET /api/projects/status/{status}` - Get projects by status

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `GET /api/tasks/project/{projectId}` - Get tasks by project
- `GET /api/tasks/status/{status}` - Get tasks by status
- `GET /api/tasks/priority/{priority}` - Get tasks by priority
- `GET /api/tasks/overdue` - Get overdue tasks

### Time Tracking
- `GET /api/timetracking` - Get all time entries
- `GET /api/timetracking/{id}` - Get time entry by ID
- `POST /api/timetracking` - Create new time entry
- `PUT /api/timetracking/{id}` - Update time entry
- `DELETE /api/timetracking/{id}` - Delete time entry
- `GET /api/timetracking/task/{taskId}` - Get time entries by task
- `GET /api/timetracking/daterange` - Get time entries by date range
- `GET /api/timetracking/task/{taskId}/total` - Get total time for task
- `GET /api/timetracking/project/{projectId}/total` - Get total time for project
- `POST /api/timetracking/start` - Start timer
- `POST /api/timetracking/stop/{timeEntryId}` - Stop timer

## 🛠️ Technologies Used

- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Primary database (with in-memory option for testing)
- **AutoMapper** - Object-to-object mapping
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop (for containerized deployment)
- SQL Server (optional, uses in-memory database by default)

### Running Locally

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ProductivityApi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   cd src/ProductivityApi
   dotnet run
   ```

4. **Access the API**
   - Swagger UI: `https://localhost:7001/swagger`
   - API Base URL: `https://localhost:7001/api`

### Running with Docker

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Access the application**
   - API: `http://localhost:5000`
   - Swagger: `http://localhost:5000/swagger`

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/ProductivityApi.Tests/
```

## 📊 Data Models

### Project Statuses
- Planning
- InProgress
- OnHold
- Completed
- Cancelled

### Task Statuses
- ToDo
- InProgress
- InReview
- Completed
- Cancelled

### Task Priorities
- Low
- Medium
- High
- Critical

## 🔧 Configuration

### Database Connection
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductivityDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Docker Configuration
The application includes Docker support with:
- Multi-stage Dockerfile for optimized builds
- Docker Compose with SQL Server integration
- Health checks and proper networking

## 📈 Sample Data

The application automatically seeds sample data including:
- 3 sample projects
- 4 sample tasks
- 3 sample time entries

## 🧪 Testing

The solution includes comprehensive tests:

- **Unit Tests**: Service layer testing with mocked dependencies
- **Integration Tests**: Full API testing with test database
- **Controller Tests**: HTTP endpoint testing

### Test Structure
```
ProductivityApi.Tests/
├── Controllers/
│   ├── ProjectsControllerTests.cs
│   └── TasksControllerTests.cs
├── Services/
│   └── ProjectServiceTests.cs
└── ProductivityApiFactory.cs    # Test fixture
```

## 📝 API Documentation

When running the application, visit `/swagger` for interactive API documentation with:
- Complete endpoint descriptions
- Request/response schemas
- Try-it-out functionality
- Authentication details

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [xUnit Documentation](https://xunit.net/)

---

*Built with ❤️ using ASP.NET Core*
