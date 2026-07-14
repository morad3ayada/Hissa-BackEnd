using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Courses.DTOs;

public class CourseDto : IMapFrom<Course>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int? DurationInMinutes { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int SectionsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Course, CourseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => $"{s.Instructor.FirstName} {s.Instructor.LastName}"))
            .ForMember(d => d.SectionsCount, o => o.MapFrom(s => s.CourseSections.Count));
    }
}
