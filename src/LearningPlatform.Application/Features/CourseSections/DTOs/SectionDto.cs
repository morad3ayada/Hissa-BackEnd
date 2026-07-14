using AutoMapper;
using LearningPlatform.Application.Common.Mappings;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.CourseSections.DTOs;

public class SectionDto : IMapFrom<CourseSection>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid CourseId { get; set; }
    public int LessonsCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CourseSection, SectionDto>()
            .ForMember(d => d.LessonsCount, o => o.MapFrom(s => s.Lessons.Count));
    }
}
