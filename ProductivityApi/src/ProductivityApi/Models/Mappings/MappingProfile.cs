using AutoMapper;
using ProductivityApi.Models;
using ProductivityApi.Models.DTOs;

namespace ProductivityApi.Models.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Project mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateProjectDto, Project>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProjectStatus>(src.Status)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<UpdateProjectDto, Project>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? Enum.Parse<ProjectStatus>(src.Status) : default(ProjectStatus?)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Task mappings
        CreateMap<ProductivityTask, ProductivityTaskDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name));

        CreateMap<CreateTaskDto, ProductivityTask>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProductivityTaskStatus>(src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => Enum.Parse<TaskPriority>(src.Priority)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.TimeEntries, opt => opt.Ignore());

        CreateMap<UpdateTaskDto, ProductivityTask>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? Enum.Parse<ProductivityTaskStatus>(src.Status) : default(ProductivityTaskStatus?)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority != null ? Enum.Parse<TaskPriority>(src.Priority) : default(TaskPriority?)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // TimeEntry mappings
        CreateMap<TimeEntry, TimeEntryDto>()
            .ForMember(dest => dest.TaskTitle, opt => opt.MapFrom(src => src.Task.Title));

        CreateMap<CreateTimeEntryDto, TimeEntry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EndTime.HasValue ? src.EndTime.Value - src.StartTime : TimeSpan.Zero))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Task, opt => opt.Ignore());

        CreateMap<UpdateTimeEntryDto, TimeEntry>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                src.EndTime.HasValue && src.StartTime.HasValue ? src.EndTime.Value - src.StartTime.Value : default(TimeSpan?)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
