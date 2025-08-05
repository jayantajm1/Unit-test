using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Services;

public class ProjectService : IProjectService
{
    private readonly ProductivityContext _context;
    private readonly IMapper _mapper;

    public ProjectService(ProductivityContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _context.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TimeEntries)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TimeEntries)
            .FirstOrDefaultAsync(p => p.Id == id);

        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        var project = _mapper.Map<Project>(createProjectDto);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return null;

        _mapper.Map(updateProjectDto, project);
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByStatusAsync(ProjectStatus status)
    {
        var projects = await _context.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TimeEntries)
            .Where(p => p.Status == status)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}
