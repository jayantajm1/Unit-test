using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto);
    Task<bool> DeleteProjectAsync(int id);
    Task<IEnumerable<ProjectDto>> GetProjectsByStatusAsync(ProjectStatus status);
}
